using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.Commands;
using Mavrynt.Modules.Notifications.Application.Queries;

namespace Mavrynt.AdminApp.Endpoints;

public static class AdminNotificationSmtpSettingsEndpoints
{
    public static IEndpointRouteBuilder MapAdminNotificationSmtpSettingsEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/notifications/smtp-settings").WithTags("Admin");

        group.MapGet("/", ListAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("ListSmtpSettings");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("GetSmtpSettingsById");

        group.MapPost("/", CreateAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("CreateSmtpSettings");

        group.MapPatch("/{id:guid}", UpdateAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("UpdateSmtpSettings");

        group.MapPatch("/{id:guid}/enable", EnableAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("EnableSmtpSettings");

        return app;
    }

    // ── Request models ─────────────────────────────────────────────────────────

    private sealed record CreateSmtpSettingsRequest(
        string ProviderName,
        string Host,
        int Port,
        string Username,
        string Password,
        string SenderEmail,
        string SenderName,
        bool UseSsl,
        bool IsEnabled);

    private sealed record UpdateSmtpSettingsRequest(
        string ProviderName,
        string Host,
        int Port,
        string Username,
        string? Password,
        string SenderEmail,
        string SenderName,
        bool UseSsl);

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<IResult> ListAsync(IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.SendAsync(new ListSmtpSettingsQuery(), ct);
        return result.IsFailure ? MapToHttpError(result.Error) : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetByIdAsync(Guid id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.SendAsync(new GetSmtpSettingsByIdQuery(id), ct);
        return result.IsFailure ? MapToHttpError(result.Error) : Results.Ok(result.Value);
    }

    private static async Task<IResult> CreateAsync(
        CreateSmtpSettingsRequest request, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.SendAsync(
            new CreateSmtpSettingsCommand(
                request.ProviderName,
                request.Host,
                request.Port,
                request.Username,
                request.Password,
                request.SenderEmail,
                request.SenderName,
                request.UseSsl,
                request.IsEnabled),
            ct);

        return result.IsFailure
            ? MapToHttpError(result.Error)
            : Results.Created($"/api/admin/notifications/smtp-settings/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateSmtpSettingsRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.SendAsync(
            new UpdateSmtpSettingsCommand(
                id,
                request.ProviderName,
                request.Host,
                request.Port,
                request.Username,
                request.Password,
                request.SenderEmail,
                request.SenderName,
                request.UseSsl),
            ct);

        return result.IsFailure ? MapToHttpError(result.Error) : Results.Ok(result.Value);
    }

    private static async Task<IResult> EnableAsync(Guid id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.SendAsync(new EnableSmtpSettingsCommand(id), ct);
        return result.IsFailure ? MapToHttpError(result.Error) : Results.Ok(result.Value);
    }

    // ── Error mapping ──────────────────────────────────────────────────────────

    private static IResult MapToHttpError(Error error)
    {
        var body = new { code = error.Code, message = error.Message };
        return error.Code switch
        {
            "Notifications.SmtpSettings.NotFound" =>
                Results.Json(body, statusCode: StatusCodes.Status404NotFound),
            _ =>
                Results.Json(body, statusCode: StatusCodes.Status400BadRequest),
        };
    }
}

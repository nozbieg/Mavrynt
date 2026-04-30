using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.Commands;
using Mavrynt.Modules.Notifications.Application.Queries;

namespace Mavrynt.AdminApp.Endpoints;

public static class AdminNotificationEmailTemplateEndpoints
{
    public static IEndpointRouteBuilder MapAdminNotificationEmailTemplateEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/notifications/email").WithTags("Admin");

        group.MapGet("/templates", ListTemplatesAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("ListEmailTemplates");

        group.MapGet("/templates/{templateKey}", GetTemplateByKeyAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("GetEmailTemplateByKey");

        group.MapPatch("/templates/{templateKey}", UpdateTemplateAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("UpdateEmailTemplate");

        group.MapGet("/template-definitions", ListDefinitionsAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("ListEmailTemplateDefinitions");

        group.MapPost("/test-send", SendTestEmailAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("SendTestEmail");

        return app;
    }

    // ── Request models ─────────────────────────────────────────────────────────

    private sealed record UpdateEmailTemplateRequest(
        string? DisplayName,
        string? Description,
        string? SubjectTemplate,
        string? HtmlBodyTemplate,
        string? TextBodyTemplate,
        bool? IsEnabled);

    private sealed record SendTestEmailRequest(string RecipientEmail, string? TemplateKey);

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<IResult> ListTemplatesAsync(IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.SendAsync(new ListEmailTemplatesQuery(), ct);
        return result.IsFailure ? MapToHttpError(result.Error) : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetTemplateByKeyAsync(
        string templateKey, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.SendAsync(new GetEmailTemplateByKeyQuery(templateKey), ct);
        return result.IsFailure ? MapToHttpError(result.Error) : Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateTemplateAsync(
        string templateKey,
        UpdateEmailTemplateRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.SendAsync(
            new UpdateEmailTemplateCommand(
                templateKey,
                request.DisplayName,
                request.Description,
                request.SubjectTemplate,
                request.HtmlBodyTemplate,
                request.TextBodyTemplate,
                request.IsEnabled),
            ct);
        return result.IsFailure ? MapToHttpError(result.Error) : Results.Ok(result.Value);
    }

    private static async Task<IResult> ListDefinitionsAsync(IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.SendAsync(new ListEmailTemplateDefinitionsQuery(), ct);
        return result.IsFailure ? MapToHttpError(result.Error) : Results.Ok(result.Value);
    }

    private static async Task<IResult> SendTestEmailAsync(
        SendTestEmailRequest request, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.SendAsync(
            new SendTestEmailCommand(request.RecipientEmail, request.TemplateKey), ct);
        return result.IsFailure ? MapToHttpError(result.Error) : Results.Ok(new { message = "Test email sent." });
    }

    // ── Error mapping ──────────────────────────────────────────────────────────

    private static IResult MapToHttpError(Error error)
    {
        var body = new { code = error.Code, message = error.Message };
        return error.Code switch
        {
            "Notifications.EmailTemplate.NotFound" =>
                Results.Json(body, statusCode: StatusCodes.Status404NotFound),
            "Notifications.EmailTemplate.KeyUnknown" =>
                Results.Json(body, statusCode: StatusCodes.Status400BadRequest),
            "Notifications.EmailTemplate.Disabled" =>
                Results.Json(body, statusCode: StatusCodes.Status400BadRequest),
            _ =>
                Results.Json(body, statusCode: StatusCodes.Status400BadRequest),
        };
    }
}

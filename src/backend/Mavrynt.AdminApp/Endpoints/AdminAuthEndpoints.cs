using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.Commands;

namespace Mavrynt.AdminApp.Endpoints;

public static class AdminAuthEndpoints
{
    public static IEndpointRouteBuilder MapAdminAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", LoginAsync)
            .AllowAnonymous()
            .WithName("AdminLoginUser");

        group.MapPost("/change-password", ChangePasswordAsync)
            .RequireAuthorization()
            .WithName("AdminChangeOwnPassword");

        return app;
    }

    // ── Request models ────────────────────────────────────────────────────────

    private sealed record LoginRequest(string Email, string Password);
    private sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.SendAsync(
            new LoginUserCommand(request.Email, request.Password),
            ct);

        return result.IsFailure
            ? MapToHttpError(result.Error)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> ChangePasswordAsync(
        ChangePasswordRequest request,
        HttpContext httpContext,
        IMediator mediator,
        CancellationToken ct)
    {
        var userIdStr = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (userIdStr is null || !Guid.TryParse(userIdStr, out var userId))
            return Results.Unauthorized();

        var result = await mediator.SendAsync(
            new ChangeOwnPasswordCommand(userId, request.CurrentPassword, request.NewPassword),
            ct);

        return result.IsFailure
            ? MapToHttpError(result.Error)
            : Results.NoContent();
    }

    // ── Error mapping ─────────────────────────────────────────────────────────

    private static IResult MapToHttpError(Error error)
    {
        var body = new { code = error.Code, message = error.Message };

        return error.Code switch
        {
            "Users.User.InvalidCredentials" =>
                Results.Json(body, statusCode: StatusCodes.Status401Unauthorized),
            "Users.User.NotFound" =>
                Results.Json(body, statusCode: StatusCodes.Status404NotFound),
            "Users.User.InvalidCurrentPassword" =>
                Results.Json(body, statusCode: StatusCodes.Status400BadRequest),
            _ =>
                Results.Json(body, statusCode: StatusCodes.Status400BadRequest),
        };
    }
}

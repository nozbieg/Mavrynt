using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.Commands;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Queries;

namespace Mavrynt.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", RegisterAsync)
            .AllowAnonymous()
            .WithName("RegisterUser");

        group.MapPost("/login", LoginAsync)
            .AllowAnonymous()
            .WithName("LoginUser");

        group.MapGet("/me", MeAsync)
            .RequireAuthorization()
            .WithName("GetCurrentUser");

        return app;
    }

    // ── Request models ────────────────────────────────────────────────────────

    private sealed record RegisterRequest(string Email, string Password, string? DisplayName);
    private sealed record LoginRequest(string Email, string Password);

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        ICommandHandler<RegisterUserCommand, UserDto> handler,
        ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var logger = loggerFactory.CreateLogger("Mavrynt.Api.Auth");

        var result = await handler.HandleAsync(
            new RegisterUserCommand(request.Email, request.Password, request.DisplayName),
            ct);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Registration failed. ErrorCode: {ErrorCode}, Email: {Email}",
                result.Error.Code, request.Email);

            return MapToHttpError(result.Error);
        }

        logger.LogInformation(
            "User registered. UserId: {UserId}",
            result.Value.Id);

        return Results.Created($"/api/users/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        ICommandHandler<LoginUserCommand, AuthResultDto> handler,
        ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var logger = loggerFactory.CreateLogger("Mavrynt.Api.Auth");

        var result = await handler.HandleAsync(
            new LoginUserCommand(request.Email, request.Password),
            ct);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Login failed. ErrorCode: {ErrorCode}, Email: {Email}",
                result.Error.Code, request.Email);

            return MapToHttpError(result.Error);
        }

        // Never log the token value — only metadata.
        logger.LogInformation(
            "Login successful. UserId: {UserId}, TokenExpiry: {Expiry}",
            result.Value.User.Id, result.Value.ExpiresAt);

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> MeAsync(
        HttpContext httpContext,
        IQueryHandler<GetUserByIdQuery, UserDto> handler,
        CancellationToken ct)
    {
        var userIdStr = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (userIdStr is null || !Guid.TryParse(userIdStr, out var userId))
            return Results.Unauthorized();

        var result = await handler.HandleAsync(new GetUserByIdQuery(userId), ct);

        return result.IsFailure
            ? MapToHttpError(result.Error)
            : Results.Ok(result.Value);
    }

    // ── Error mapping ─────────────────────────────────────────────────────────

    private static IResult MapToHttpError(Error error)
    {
        var body = new { code = error.Code, message = error.Message };

        return error.Code switch
        {
            "Users.User.InvalidCredentials" =>
                Results.Json(body, statusCode: StatusCodes.Status401Unauthorized),
            "Users.User.EmailAlreadyTaken" =>
                Results.Json(body, statusCode: StatusCodes.Status409Conflict),
            "Users.User.NotFound" =>
                Results.Json(body, statusCode: StatusCodes.Status404NotFound),
            _ =>
                Results.Json(body, statusCode: StatusCodes.Status400BadRequest),
        };
    }
}

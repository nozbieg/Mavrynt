using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Queries;

namespace Mavrynt.AdminApp.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin").WithTags("Admin");

        group.MapGet("/me", MeAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("GetCurrentAdmin");

        return app;
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<IResult> MeAsync(
        HttpContext httpContext,
        IMediator mediator,
        CancellationToken ct)
    {
        var userIdStr = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (userIdStr is null || !Guid.TryParse(userIdStr, out var userId))
            return Results.Unauthorized();

        var result = await mediator.SendAsync(new GetUserByIdQuery(userId), ct);

        return result.IsFailure
            ? Results.Problem(detail: result.Error.Message, statusCode: StatusCodes.Status404NotFound)
            : Results.Ok(result.Value);
    }
}

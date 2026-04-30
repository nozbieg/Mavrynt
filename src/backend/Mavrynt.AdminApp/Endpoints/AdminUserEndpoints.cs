using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.Commands;
using Mavrynt.Modules.Users.Application.DTOs;

namespace Mavrynt.AdminApp.Endpoints;

public static class AdminUserEndpoints
{
    public static IEndpointRouteBuilder MapAdminUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/users").WithTags("Admin");

        group.MapPatch("/{userId:guid}/role", AssignRoleAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("AssignUserRole");

        return app;
    }

    private sealed record AssignRoleRequest(string Role);

    private static async Task<IResult> AssignRoleAsync(
        Guid userId,
        AssignRoleRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.SendAsync(
            new AssignUserRoleCommand(userId, request.Role),
            ct);

        return result.IsFailure
            ? MapToHttpError(result.Error)
            : Results.Ok(result.Value);
    }

    private static IResult MapToHttpError(Error error)
    {
        var body = new { code = error.Code, message = error.Message };

        return error.Code switch
        {
            "Users.User.NotFound" =>
                Results.Json(body, statusCode: StatusCodes.Status404NotFound),
            "Users.User.InvalidRole" =>
                Results.Json(body, statusCode: StatusCodes.Status400BadRequest),
            _ =>
                Results.Json(body, statusCode: StatusCodes.Status400BadRequest),
        };
    }
}

using System.IdentityModel.Tokens.Jwt;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.FeatureManagement.Application.Commands;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;
using Mavrynt.Modules.FeatureManagement.Application.Queries;

namespace Mavrynt.AdminApp.Endpoints;

public static class AdminFeatureFlagEndpoints
{
    public static IEndpointRouteBuilder MapAdminFeatureFlagEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/feature-flags").WithTags("Admin");

        group.MapGet("/", ListAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("ListFeatureFlags");

        group.MapGet("/{key}", GetByKeyAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("GetFeatureFlagByKey");

        group.MapPost("/", CreateAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("CreateFeatureFlag");

        group.MapPatch("/{key}", UpdateAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("UpdateFeatureFlag");

        group.MapPatch("/{key}/toggle", ToggleAsync)
            .RequireAuthorization("AdminOnly")
            .WithName("ToggleFeatureFlag");

        return app;
    }

    // ── Request models ─────────────────────────────────────────────────────────

    private sealed record CreateFeatureFlagRequest(
        string Key,
        string Name,
        string? Description,
        bool IsEnabled);

    private sealed record UpdateFeatureFlagRequest(
        string Name,
        string? Description);

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<IResult> ListAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.SendAsync(new ListFeatureFlagsQuery(), ct);
        return result.IsFailure
            ? MapToHttpError(result.Error)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetByKeyAsync(
        string key,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.SendAsync(new GetFeatureFlagByKeyQuery(key), ct);
        return result.IsFailure
            ? MapToHttpError(result.Error)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> CreateAsync(
        CreateFeatureFlagRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.SendAsync(
            new CreateFeatureFlagCommand(request.Key, request.Name, request.Description, request.IsEnabled),
            ct);

        return result.IsFailure
            ? MapToHttpError(result.Error)
            : Results.Created($"/api/admin/feature-flags/{result.Value.Key}", result.Value);
    }

    private static async Task<IResult> UpdateAsync(
        string key,
        UpdateFeatureFlagRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.SendAsync(
            new UpdateFeatureFlagCommand(key, request.Name, request.Description),
            ct);

        return result.IsFailure
            ? MapToHttpError(result.Error)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> ToggleAsync(
        string key,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.SendAsync(new ToggleFeatureFlagCommand(key), ct);
        return result.IsFailure
            ? MapToHttpError(result.Error)
            : Results.Ok(result.Value);
    }

    // ── Error mapping ──────────────────────────────────────────────────────────

    private static IResult MapToHttpError(Error error)
    {
        var body = new { code = error.Code, message = error.Message };

        return error.Code switch
        {
            "FeatureManagement.FeatureFlag.NotFound" =>
                Results.Json(body, statusCode: StatusCodes.Status404NotFound),
            "FeatureManagement.FeatureFlag.KeyAlreadyTaken" =>
                Results.Json(body, statusCode: StatusCodes.Status409Conflict),
            _ =>
                Results.Json(body, statusCode: StatusCodes.Status400BadRequest),
        };
    }
}

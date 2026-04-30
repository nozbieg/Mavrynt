namespace Mavrynt.Modules.FeatureManagement.Application.DTOs;

public sealed record FeatureFlagDto(
    Guid Id,
    string Key,
    string Name,
    string? Description,
    bool IsEnabled,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);

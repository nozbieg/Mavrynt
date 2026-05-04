using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;
using Mavrynt.Modules.FeatureManagement.Application.Queries;

namespace Mavrynt.Modules.FeatureManagement.Application.Commands;

public sealed record CreateFeatureFlagCommand(
    string Key,
    string Name,
    string? Description,
    bool IsEnabled
) : ICommand<FeatureFlagDto>, ITransactionalRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheKeysToInvalidate => [FeatureManagementCacheKeys.FeatureFlagByKey(Key), FeatureManagementCacheKeys.FeatureFlagsList];
    public IReadOnlyCollection<string> CacheTagsToInvalidate => ["feature-management:feature-flags", $"feature-management:feature-flag:{Key.Trim().ToLowerInvariant()}"];
}

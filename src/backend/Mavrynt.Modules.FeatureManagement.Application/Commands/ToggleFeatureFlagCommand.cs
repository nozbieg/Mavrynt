using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.FeatureManagement.Application.Queries;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;

namespace Mavrynt.Modules.FeatureManagement.Application.Commands;

public sealed record ToggleFeatureFlagCommand(string Key) : ICommand<FeatureFlagDto>, ITransactionalRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheKeysToInvalidate => [FeatureManagementCacheKeys.FeatureFlagByKey(Key), FeatureManagementCacheKeys.FeatureFlagsList];
    public IReadOnlyCollection<string> CacheTagsToInvalidate => ["feature-management:feature-flags", $"feature-management:feature-flag:{Key.Trim().ToLowerInvariant()}"];
}

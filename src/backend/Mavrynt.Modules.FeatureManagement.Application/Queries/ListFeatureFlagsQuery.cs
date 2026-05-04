using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;

namespace Mavrynt.Modules.FeatureManagement.Application.Queries;

public sealed record ListFeatureFlagsQuery : ICachedQuery<IReadOnlyList<FeatureFlagDto>>
{
    public string CacheKey => FeatureManagementCacheKeys.FeatureFlagsList;
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(1);
    public IReadOnlyCollection<string> CacheTags => ["feature-management:feature-flags"];
}

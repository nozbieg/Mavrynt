using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;

namespace Mavrynt.Modules.FeatureManagement.Application.Queries;

public sealed record GetFeatureFlagByKeyQuery(string Key) : ICachedQuery<FeatureFlagDto>
{
    public string CacheKey => FeatureManagementCacheKeys.FeatureFlagByKey(Key);
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheTags => ["feature-management:feature-flags", $"feature-management:feature-flag:{Key.Trim().ToLowerInvariant()}"];
}

namespace Mavrynt.Modules.FeatureManagement.Application.Queries;

public static class FeatureManagementCacheKeys
{
    public static string FeatureFlagByKey(string key) => $"feature-management:feature-flag:key:{key.Trim().ToLowerInvariant()}";
    public const string FeatureFlagsList = "feature-management:feature-flags:list:all";
}

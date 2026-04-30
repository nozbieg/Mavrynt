using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;

namespace Mavrynt.Modules.FeatureManagement.Domain.Errors;

public static class FeatureManagementErrors
{
    public static readonly Error InvalidFeatureFlagId =
        new("FeatureManagement.FeatureFlagId.Invalid", "Feature flag ID must not be an empty GUID.");

    public static readonly Error KeyEmpty =
        new("FeatureManagement.Key.Empty", "Feature flag key must not be empty.");

    public static readonly Error KeyTooLong =
        new("FeatureManagement.Key.TooLong", $"Feature flag key must not exceed {FeatureFlagKey.MaxLength} characters.");

    public static readonly Error KeyInvalid =
        new("FeatureManagement.Key.Invalid",
            "Feature flag key may only contain lowercase letters, digits, dots, dashes, and underscores, and must start with a letter or digit.");

    public static readonly Error NameEmpty =
        new("FeatureManagement.Name.Empty", "Feature flag name must not be empty.");

    public static readonly Error NameTooLong =
        new("FeatureManagement.Name.TooLong", "Feature flag name must not exceed 256 characters.");

    public static readonly Error FeatureFlagNotFound =
        new("FeatureManagement.FeatureFlag.NotFound", "The requested feature flag was not found.");

    public static readonly Error KeyAlreadyTaken =
        new("FeatureManagement.FeatureFlag.KeyAlreadyTaken", "A feature flag with the given key already exists.");
}

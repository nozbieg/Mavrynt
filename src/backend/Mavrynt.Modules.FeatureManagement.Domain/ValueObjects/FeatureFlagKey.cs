using System.Text.RegularExpressions;
using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.FeatureManagement.Domain.Errors;

namespace Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;

public sealed class FeatureFlagKey : ValueObject
{
    public const int MaxLength = 256;

    private static readonly Regex ValidPattern =
        new(@"^[a-z0-9][a-z0-9._-]*$", RegexOptions.Compiled);

    public string Value { get; }

    private FeatureFlagKey(string value) => Value = value;

    public static Result<FeatureFlagKey> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return FeatureManagementErrors.KeyEmpty;

        value = value.Trim();

        if (value.Length > MaxLength)
            return FeatureManagementErrors.KeyTooLong;

        if (!ValidPattern.IsMatch(value))
            return FeatureManagementErrors.KeyInvalid;

        return new FeatureFlagKey(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}

using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.FeatureManagement.Domain.Errors;

namespace Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;

public sealed class FeatureFlagId : ValueObject
{
    public Guid Value { get; }

    private FeatureFlagId(Guid value) => Value = value;

    public static Result<FeatureFlagId> New() => From(Guid.NewGuid());

    public static Result<FeatureFlagId> From(Guid value)
    {
        if (value == Guid.Empty)
            return FeatureManagementErrors.InvalidFeatureFlagId;

        return new FeatureFlagId(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}

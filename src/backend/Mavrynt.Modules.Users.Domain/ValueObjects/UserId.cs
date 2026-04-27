using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Domain.Errors;

namespace Mavrynt.Modules.Users.Domain.ValueObjects;

public sealed class UserId : ValueObject
{
    private UserId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static Result<UserId> New()
        => From(Guid.NewGuid());

    public static Result<UserId> From(Guid value)
    {
        if (value == Guid.Empty)
            return UserErrors.InvalidUserId;

        return new UserId(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}

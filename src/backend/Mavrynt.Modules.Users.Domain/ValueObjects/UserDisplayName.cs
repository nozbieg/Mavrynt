using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Domain.Errors;

namespace Mavrynt.Modules.Users.Domain.ValueObjects;

public sealed class UserDisplayName : ValueObject
{
    public const int MaxLength = 120;

    private UserDisplayName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<UserDisplayName> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return UserErrors.DisplayNameEmpty;

        if (value.Length > MaxLength)
            return UserErrors.DisplayNameTooLong;

        return new UserDisplayName(value.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

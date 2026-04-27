using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Domain.Errors;

namespace Mavrynt.Modules.Users.Domain.ValueObjects;

public sealed class PasswordHash : ValueObject
{
    public const int MaxLength = 1024;

    private PasswordHash(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    /// Creates a <see cref="PasswordHash"/> from an already-hashed password string.
    /// Does not perform hashing itself.
    /// </summary>
    public static Result<PasswordHash> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return UserErrors.PasswordHashEmpty;

        if (value.Length > MaxLength)
            return UserErrors.PasswordHashTooLong;

        return new PasswordHash(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => "[protected]";
}

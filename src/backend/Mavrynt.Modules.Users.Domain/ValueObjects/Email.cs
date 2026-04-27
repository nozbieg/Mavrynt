using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Domain.Errors;

namespace Mavrynt.Modules.Users.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public const int MaxLength = 254;

    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Email> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return UserErrors.EmailEmpty;

        if (value.Length > MaxLength)
            return UserErrors.EmailTooLong;

        var normalized = value.Trim().ToLowerInvariant();

        var atIndex = normalized.IndexOf('@');
        if (atIndex <= 0 || atIndex == normalized.Length - 1 || normalized.IndexOf('.', atIndex) == -1)
            return UserErrors.EmailInvalid;

        return new Email(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

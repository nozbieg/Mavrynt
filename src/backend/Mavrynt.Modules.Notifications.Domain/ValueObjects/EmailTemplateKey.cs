using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Domain.Errors;

namespace Mavrynt.Modules.Notifications.Domain.ValueObjects;

public sealed class EmailTemplateKey : ValueObject
{
    public const int MaxLength = 128;

    // Predefined template keys — the only values accepted by the system.
    public const string LoginConfirmation = "auth.login_confirmation";
    public const string PasswordReset = "auth.password_reset";
    public const string TwoFactorCode = "auth.two_factor_code";

    private static readonly HashSet<string> ValidKeys =
    [
        LoginConfirmation,
        PasswordReset,
        TwoFactorCode,
    ];

    public string Value { get; }

    private EmailTemplateKey(string value) => Value = value;

    public static Result<EmailTemplateKey> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return NotificationsErrors.EmailTemplateKeyEmpty;

        value = value.Trim();

        if (value.Length > MaxLength)
            return NotificationsErrors.EmailTemplateKeyTooLong;

        if (!ValidKeys.Contains(value))
            return NotificationsErrors.EmailTemplateKeyUnknown;

        return new EmailTemplateKey(value);
    }

    public static IReadOnlySet<string> AllKeys => ValidKeys;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}

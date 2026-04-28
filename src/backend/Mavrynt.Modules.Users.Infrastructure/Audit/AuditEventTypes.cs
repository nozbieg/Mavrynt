namespace Mavrynt.Modules.Users.Infrastructure.Audit;

/// <summary>
/// Well-known audit event type strings for the Users module.
/// Kept as constants so callers don't scatter magic strings.
/// </summary>
public static class AuditEventTypes
{
    public const string UserRegistered = "user_registered";
    public const string LoginSuccess = "login_success";
    public const string LoginFailed = "login_failed";
    public const string PasswordChanged = "password_changed";
    public const string EmailChanged = "email_changed";
}

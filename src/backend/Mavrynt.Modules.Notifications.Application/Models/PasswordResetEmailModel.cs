namespace Mavrynt.Modules.Notifications.Application.Models;

public sealed record PasswordResetEmailModel(
    string UserEmail,
    string? DisplayName,
    string ResetLink,
    DateTimeOffset ExpiresAt
) : IEmailModel
{
    public IReadOnlyDictionary<string, string> ToPlaceholders() =>
        new Dictionary<string, string>
        {
            ["UserEmail"] = UserEmail,
            ["DisplayName"] = DisplayName ?? string.Empty,
            ["ResetLink"] = ResetLink,
            ["ExpiresAt"] = ExpiresAt.ToString("yyyy-MM-dd HH:mm:ss UTC"),
        };
}

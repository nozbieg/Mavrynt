namespace Mavrynt.Modules.Notifications.Application.Models;

public sealed record TwoFactorCodeEmailModel(
    string UserEmail,
    string? DisplayName,
    string Code,
    DateTimeOffset ExpiresAt
) : IEmailModel
{
    public IReadOnlyDictionary<string, string> ToPlaceholders() =>
        new Dictionary<string, string>
        {
            ["UserEmail"] = UserEmail,
            ["DisplayName"] = DisplayName ?? string.Empty,
            ["Code"] = Code,
            ["ExpiresAt"] = ExpiresAt.ToString("yyyy-MM-dd HH:mm:ss UTC"),
        };
}

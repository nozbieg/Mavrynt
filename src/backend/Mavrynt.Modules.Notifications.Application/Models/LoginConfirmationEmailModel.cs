namespace Mavrynt.Modules.Notifications.Application.Models;

public sealed record LoginConfirmationEmailModel(
    string UserEmail,
    string? DisplayName,
    DateTimeOffset LoginAt,
    string? IpAddress,
    string? UserAgent
) : IEmailModel
{
    public IReadOnlyDictionary<string, string> ToPlaceholders() =>
        new Dictionary<string, string>
        {
            ["UserEmail"] = UserEmail,
            ["DisplayName"] = DisplayName ?? string.Empty,
            ["LoginAt"] = LoginAt.ToString("yyyy-MM-dd HH:mm:ss UTC"),
            ["IpAddress"] = IpAddress ?? string.Empty,
            ["UserAgent"] = UserAgent ?? string.Empty,
        };
}

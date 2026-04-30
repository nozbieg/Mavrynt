namespace Mavrynt.Modules.Notifications.Application.DTOs;

public sealed record SmtpSettingsDto(
    Guid Id,
    string ProviderName,
    string Host,
    int Port,
    string Username,
    string SenderEmail,
    string SenderName,
    bool UseSsl,
    bool IsEnabled,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);

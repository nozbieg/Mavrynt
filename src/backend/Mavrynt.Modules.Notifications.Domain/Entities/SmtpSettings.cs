using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Domain.Entities;

public sealed class SmtpSettings : AggregateRoot<SmtpSettingsId>
{
    private SmtpSettings() : base(null!) { }

    private SmtpSettings(
        SmtpSettingsId id,
        string providerName,
        string host,
        int port,
        string username,
        string protectedPassword,
        string senderEmail,
        string senderName,
        bool useSsl,
        bool isEnabled,
        DateTimeOffset createdAt)
        : base(id)
    {
        ProviderName = providerName;
        Host = host;
        Port = port;
        Username = username;
        ProtectedPassword = protectedPassword;
        SenderEmail = senderEmail;
        SenderName = senderName;
        UseSsl = useSsl;
        IsEnabled = isEnabled;
        CreatedAt = createdAt;
    }

    public string ProviderName { get; private set; } = string.Empty;
    public string Host { get; private set; } = string.Empty;
    public int Port { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string ProtectedPassword { get; private set; } = string.Empty;
    public string SenderEmail { get; private set; } = string.Empty;
    public string SenderName { get; private set; } = string.Empty;
    public bool UseSsl { get; private set; }
    public bool IsEnabled { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Result<SmtpSettings> Create(
        SmtpSettingsId id,
        string providerName,
        string host,
        int port,
        string username,
        string protectedPassword,
        string senderEmail,
        string senderName,
        bool useSsl,
        bool isEnabled,
        DateTimeOffset createdAt)
    {
        var validationError = Validate(providerName, host, port, username, protectedPassword, senderEmail);
        if (validationError is not null)
            return validationError;

        return new SmtpSettings(
            id, providerName.Trim(), host.Trim(), port,
            username.Trim(), protectedPassword,
            senderEmail.Trim(), senderName.Trim(),
            useSsl, isEnabled, createdAt);
    }

    public Result Update(
        string providerName,
        string host,
        int port,
        string username,
        string? protectedPassword,
        string senderEmail,
        string senderName,
        bool useSsl,
        DateTimeOffset updatedAt)
    {
        var passwordToUse = protectedPassword ?? ProtectedPassword;
        var validationError = Validate(providerName, host, port, username, passwordToUse, senderEmail);
        if (validationError is not null)
            return validationError;

        ProviderName = providerName.Trim();
        Host = host.Trim();
        Port = port;
        Username = username.Trim();
        if (protectedPassword is not null)
            ProtectedPassword = protectedPassword;
        SenderEmail = senderEmail.Trim();
        SenderName = senderName.Trim();
        UseSsl = useSsl;
        UpdatedAt = updatedAt;

        return Result.Success();
    }

    public Result Enable(DateTimeOffset updatedAt)
    {
        IsEnabled = true;
        UpdatedAt = updatedAt;
        return Result.Success();
    }

    public Result Disable(DateTimeOffset updatedAt)
    {
        IsEnabled = false;
        UpdatedAt = updatedAt;
        return Result.Success();
    }

    private static Error? Validate(
        string providerName, string host, int port,
        string username, string protectedPassword, string senderEmail)
    {
        if (string.IsNullOrWhiteSpace(providerName)) return NotificationsErrors.SmtpSettingsProviderNameEmpty;
        if (string.IsNullOrWhiteSpace(host)) return NotificationsErrors.SmtpSettingsHostEmpty;
        if (port is < 1 or > 65535) return NotificationsErrors.SmtpSettingsPortInvalid;
        if (string.IsNullOrWhiteSpace(username)) return NotificationsErrors.SmtpSettingsUsernameEmpty;
        if (string.IsNullOrWhiteSpace(protectedPassword)) return NotificationsErrors.SmtpSettingsPasswordEmpty;
        if (string.IsNullOrWhiteSpace(senderEmail)) return NotificationsErrors.SmtpSettingsSenderEmailEmpty;
        return null;
    }
}

using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.Modules.Notifications.Domain.Errors;

public static class NotificationsErrors
{
    // SmtpSettings IDs
    public static readonly Error InvalidSmtpSettingsId =
        new("Notifications.SmtpSettings.InvalidId", "SMTP settings ID must not be an empty GUID.");

    // EmailTemplate IDs
    public static readonly Error InvalidEmailTemplateId =
        new("Notifications.EmailTemplate.InvalidId", "Email template ID must not be an empty GUID.");

    // EmailTemplateKey
    public static readonly Error EmailTemplateKeyEmpty =
        new("Notifications.EmailTemplate.KeyEmpty", "Email template key must not be empty.");

    public static readonly Error EmailTemplateKeyTooLong =
        new("Notifications.EmailTemplate.KeyTooLong",
            $"Email template key must not exceed {ValueObjects.EmailTemplateKey.MaxLength} characters.");

    public static readonly Error EmailTemplateKeyUnknown =
        new("Notifications.EmailTemplate.KeyUnknown",
            "The provided email template key is not a recognized predefined template type.");

    // EmailTemplate entity
    public static readonly Error EmailTemplateNotFound =
        new("Notifications.EmailTemplate.NotFound", "The requested email template was not found.");

    public static readonly Error EmailTemplateDisabled =
        new("Notifications.EmailTemplate.Disabled", "The email template is disabled and cannot be used for sending.");

    public static readonly Error EmailTemplateDisplayNameEmpty =
        new("Notifications.EmailTemplate.DisplayNameEmpty", "Email template display name must not be empty.");

    public static readonly Error EmailTemplateSubjectEmpty =
        new("Notifications.EmailTemplate.SubjectEmpty", "Email template subject must not be empty.");

    public static readonly Error EmailTemplateHtmlBodyEmpty =
        new("Notifications.EmailTemplate.HtmlBodyEmpty", "Email template HTML body must not be empty.");

    // SmtpSettings entity
    public static readonly Error SmtpSettingsNotFound =
        new("Notifications.SmtpSettings.NotFound", "The requested SMTP settings were not found.");

    public static readonly Error SmtpSettingsNoActiveProvider =
        new("Notifications.SmtpSettings.NoActiveProvider",
            "No active SMTP provider is configured. Enable an SMTP configuration before sending emails.");

    public static readonly Error SmtpSettingsHostEmpty =
        new("Notifications.SmtpSettings.HostEmpty", "SMTP host must not be empty.");

    public static readonly Error SmtpSettingsProviderNameEmpty =
        new("Notifications.SmtpSettings.ProviderNameEmpty", "SMTP provider name must not be empty.");

    public static readonly Error SmtpSettingsSenderEmailEmpty =
        new("Notifications.SmtpSettings.SenderEmailEmpty", "SMTP sender email must not be empty.");

    public static readonly Error SmtpSettingsUsernameEmpty =
        new("Notifications.SmtpSettings.UsernameEmpty", "SMTP username must not be empty.");

    public static readonly Error SmtpSettingsPasswordEmpty =
        new("Notifications.SmtpSettings.PasswordEmpty", "SMTP password must not be empty.");

    public static readonly Error SmtpSettingsPortInvalid =
        new("Notifications.SmtpSettings.PortInvalid", "SMTP port must be between 1 and 65535.");

    // Email sending
    public static readonly Error EmailSendFailed =
        new("Notifications.Email.SendFailed", "Failed to send the email. See inner details for more information.");

    public static readonly Error EmailRecipientInvalid =
        new("Notifications.Email.RecipientInvalid", "Recipient email is not a valid address.");

    public static Error EmailUnknownPlaceholder(string placeholders) =>
        new("Notifications.Email.UnknownPlaceholder",
            $"The email template contains unknown placeholders: {placeholders}.");
}

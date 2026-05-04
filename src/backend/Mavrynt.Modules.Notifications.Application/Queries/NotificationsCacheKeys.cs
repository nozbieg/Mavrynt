namespace Mavrynt.Modules.Notifications.Application.Queries;

public static class NotificationsCacheKeys
{
    public static string SmtpSettingsById(Guid id) => $"notifications:smtp-settings:id:{id:N}";
    public const string SmtpSettingsList = "notifications:smtp-settings:list:all";
    public static string EmailTemplateByKey(string key) => $"notifications:email-template:key:{key.Trim().ToLowerInvariant()}";
    public const string EmailTemplatesList = "notifications:email-templates:list:all";
    public const string EmailTemplateDefinitionsList = "notifications:email-template-definitions:list:all";
}

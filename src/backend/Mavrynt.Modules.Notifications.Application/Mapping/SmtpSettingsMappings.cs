using Mavrynt.Modules.Notifications.Application.DTOs;
using Mavrynt.Modules.Notifications.Domain.Entities;

namespace Mavrynt.Modules.Notifications.Application.Mapping;

internal static class SmtpSettingsMappings
{
    internal static SmtpSettingsDto ToDto(this SmtpSettings settings) =>
        new(
            settings.Id.Value,
            settings.ProviderName,
            settings.Host,
            settings.Port,
            settings.Username,
            settings.SenderEmail,
            settings.SenderName,
            settings.UseSsl,
            settings.IsEnabled,
            settings.CreatedAt,
            settings.UpdatedAt
        );
}

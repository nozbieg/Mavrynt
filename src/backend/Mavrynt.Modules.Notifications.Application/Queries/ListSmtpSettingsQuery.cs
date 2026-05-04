using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.Modules.Notifications.Application.DTOs;

namespace Mavrynt.Modules.Notifications.Application.Queries;

public sealed record ListSmtpSettingsQuery : ICachedQuery<IReadOnlyList<SmtpSettingsDto>>
{
    public string CacheKey => NotificationsCacheKeys.SmtpSettingsList;
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheTags => ["notifications:smtp-settings"];
}

using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.Modules.Notifications.Application.DTOs;

namespace Mavrynt.Modules.Notifications.Application.Queries;

public sealed record GetSmtpSettingsByIdQuery(Guid Id) : ICachedQuery<SmtpSettingsDto>
{
    public string CacheKey => NotificationsCacheKeys.SmtpSettingsById(Id);
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheTags => ["notifications:smtp-settings", $"notifications:smtp-settings:{Id:N}"];
}

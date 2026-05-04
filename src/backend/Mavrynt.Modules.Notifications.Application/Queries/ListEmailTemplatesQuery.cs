using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.Modules.Notifications.Application.DTOs;

namespace Mavrynt.Modules.Notifications.Application.Queries;

public sealed record ListEmailTemplatesQuery : ICachedQuery<IReadOnlyList<EmailTemplateDto>>
{
    public string CacheKey => NotificationsCacheKeys.EmailTemplatesList;
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheTags => ["notifications:email-templates"];
}

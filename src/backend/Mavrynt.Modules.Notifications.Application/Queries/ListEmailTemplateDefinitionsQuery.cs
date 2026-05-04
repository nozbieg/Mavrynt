using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.Modules.Notifications.Application.DTOs;

namespace Mavrynt.Modules.Notifications.Application.Queries;

public sealed record ListEmailTemplateDefinitionsQuery : ICachedQuery<IReadOnlyList<EmailTemplateDefinitionDto>>
{
    public string CacheKey => NotificationsCacheKeys.EmailTemplateDefinitionsList;
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheTags => ["notifications:email-templates"];
}

using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.Modules.Notifications.Application.DTOs;

namespace Mavrynt.Modules.Notifications.Application.Queries;

public sealed record GetEmailTemplateByKeyQuery(string TemplateKey) : ICachedQuery<EmailTemplateDto>
{
    public string CacheKey => NotificationsCacheKeys.EmailTemplateByKey(TemplateKey);
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheTags => ["notifications:email-templates", $"notifications:email-template:{TemplateKey.Trim().ToLowerInvariant()}"];
}

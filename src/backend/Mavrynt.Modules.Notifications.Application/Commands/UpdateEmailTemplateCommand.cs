using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Notifications.Application.Queries;
using Mavrynt.Modules.Notifications.Application.DTOs;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed record UpdateEmailTemplateCommand(
    string TemplateKey,
    string? DisplayName,
    string? Description,
    string? SubjectTemplate,
    string? HtmlBodyTemplate,
    string? TextBodyTemplate,
    bool? IsEnabled
) : ICommand<EmailTemplateDto>, ITransactionalRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheKeysToInvalidate => [NotificationsCacheKeys.EmailTemplateByKey(TemplateKey), NotificationsCacheKeys.EmailTemplatesList, NotificationsCacheKeys.EmailTemplateDefinitionsList];
    public IReadOnlyCollection<string> CacheTagsToInvalidate => ["notifications:email-templates", $"notifications:email-template:{TemplateKey.Trim().ToLowerInvariant()}"];
}

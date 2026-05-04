using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Notifications.Application.Queries;
using Mavrynt.Modules.Notifications.Application.DTOs;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed record UpdateSmtpSettingsCommand(
    Guid Id,
    string ProviderName,
    string Host,
    int Port,
    string Username,
    string? Password,
    string SenderEmail,
    string SenderName,
    bool UseSsl
) : ICommand<SmtpSettingsDto>, ITransactionalRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheKeysToInvalidate => [NotificationsCacheKeys.SmtpSettingsById(Id), NotificationsCacheKeys.SmtpSettingsList];
    public IReadOnlyCollection<string> CacheTagsToInvalidate => ["notifications:smtp-settings", $"notifications:smtp-settings:{Id:N}"];
}

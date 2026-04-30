using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.Models;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Application.Abstractions;

public interface IEmailNotificationService
{
    Task<Result> SendAsync<TModel>(
        EmailTemplateKey templateKey,
        EmailRecipient recipient,
        TModel model,
        CancellationToken cancellationToken = default)
        where TModel : IEmailModel;
}

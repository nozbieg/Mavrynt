using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Domain.Repositories;

public interface IEmailTemplateRepository
{
    Task<EmailTemplate?> GetByIdAsync(EmailTemplateId id, CancellationToken cancellationToken = default);
    Task<EmailTemplate?> GetByKeyAsync(EmailTemplateKey key, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailTemplate>> ListAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByKeyAsync(EmailTemplateKey key, CancellationToken cancellationToken = default);
    Task AddAsync(EmailTemplate template, CancellationToken cancellationToken = default);
}

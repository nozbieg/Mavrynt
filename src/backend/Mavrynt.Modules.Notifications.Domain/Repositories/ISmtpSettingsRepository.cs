using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Domain.Repositories;

public interface ISmtpSettingsRepository
{
    Task<SmtpSettings?> GetByIdAsync(SmtpSettingsId id, CancellationToken cancellationToken = default);
    Task<SmtpSettings?> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SmtpSettings>> ListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(SmtpSettings settings, CancellationToken cancellationToken = default);
    Task DisableAllAsync(CancellationToken cancellationToken = default);
}

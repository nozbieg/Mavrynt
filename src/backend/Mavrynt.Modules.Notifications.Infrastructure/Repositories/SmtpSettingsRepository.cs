using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Mavrynt.Modules.Notifications.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mavrynt.Modules.Notifications.Infrastructure.Repositories;

internal sealed class SmtpSettingsRepository : ISmtpSettingsRepository
{
    private readonly NotificationsDbContext _context;

    public SmtpSettingsRepository(NotificationsDbContext context) => _context = context;

    public Task<SmtpSettings?> GetByIdAsync(SmtpSettingsId id, CancellationToken cancellationToken = default)
        => _context.SmtpSettings.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<SmtpSettings?> GetActiveAsync(CancellationToken cancellationToken = default)
        => _context.SmtpSettings.FirstOrDefaultAsync(s => s.IsEnabled, cancellationToken);

    public async Task<IReadOnlyList<SmtpSettings>> ListAsync(CancellationToken cancellationToken = default)
        => await _context.SmtpSettings.OrderByDescending(s => s.CreatedAt).ToListAsync(cancellationToken);

    public async Task AddAsync(SmtpSettings settings, CancellationToken cancellationToken = default)
    {
        await _context.SmtpSettings.AddAsync(settings, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DisableAllAsync(CancellationToken cancellationToken = default)
    {
        var enabledSettings = await _context.SmtpSettings
            .Where(s => s.IsEnabled)
            .ToListAsync(cancellationToken);

        foreach (var s in enabledSettings)
            s.Disable(DateTimeOffset.UtcNow);
    }
}

using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Mavrynt.Modules.Notifications.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mavrynt.Modules.Notifications.Infrastructure.Repositories;

internal sealed class EmailTemplateRepository : IEmailTemplateRepository
{
    private readonly NotificationsDbContext _context;

    public EmailTemplateRepository(NotificationsDbContext context) => _context = context;

    public Task<EmailTemplate?> GetByIdAsync(EmailTemplateId id, CancellationToken cancellationToken = default)
        => _context.EmailTemplates.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<EmailTemplate?> GetByKeyAsync(EmailTemplateKey key, CancellationToken cancellationToken = default)
        => _context.EmailTemplates.FirstOrDefaultAsync(t => t.Key == key, cancellationToken);

    public async Task<IReadOnlyList<EmailTemplate>> ListAsync(CancellationToken cancellationToken = default)
        => await _context.EmailTemplates.OrderBy(t => t.Key).ToListAsync(cancellationToken);

    public Task<bool> ExistsByKeyAsync(EmailTemplateKey key, CancellationToken cancellationToken = default)
        => _context.EmailTemplates.AnyAsync(t => t.Key == key, cancellationToken);

    public async Task AddAsync(EmailTemplate template, CancellationToken cancellationToken = default)
        => await _context.EmailTemplates.AddAsync(template, cancellationToken);
}

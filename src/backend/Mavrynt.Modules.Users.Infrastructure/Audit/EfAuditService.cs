using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.Modules.Users.Infrastructure.Persistence;

namespace Mavrynt.Modules.Users.Infrastructure.Audit;

/// <summary>
/// Persists audit events to PostgreSQL via <see cref="UsersDbContext"/>.
/// Each call is its own DB write. Extraction to a dedicated Audit module
/// is straightforward: move this file and inject a standalone AuditDbContext instead.
/// </summary>
internal sealed class EfAuditService(UsersDbContext context) : IAuditService
{
    public async Task RecordAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        var auditEvent = new AuditEvent(
            id: Guid.NewGuid(),
            eventType: entry.EventType,
            occurredAt: entry.OccurredAt,
            userId: entry.UserId,
            email: entry.Email,
            source: entry.Source,
            ipAddress: entry.IpAddress,
            userAgent: entry.UserAgent,
            metadata: entry.Metadata);

        await context.AuditEvents.AddAsync(auditEvent, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}

using Mavrynt.Modules.Audit.Application.Abstractions;
using Mavrynt.Modules.Audit.Domain.Entities;
using Mavrynt.Modules.Audit.Infrastructure.Persistence;

namespace Mavrynt.Modules.Audit.Infrastructure.Repositories;

internal sealed class EfAuditLogWriter(AuditDbContext context) : IAuditLogWriter
{
    public async Task WriteAsync(
        Guid? actorUserId,
        string action,
        string resourceType,
        string? resourceId,
        string? metadataJson = null,
        CancellationToken cancellationToken = default)
    {
        var entry = AuditLogEntry.Create(
            actorUserId,
            action,
            resourceType,
            resourceId,
            DateTimeOffset.UtcNow,
            metadataJson);

        await context.AuditLogEntries.AddAsync(entry, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}

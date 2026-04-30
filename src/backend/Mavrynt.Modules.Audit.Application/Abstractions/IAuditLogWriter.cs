namespace Mavrynt.Modules.Audit.Application.Abstractions;

/// <summary>
/// Persists administrative and system audit entries.
/// Implemented in Mavrynt.Modules.Audit.Infrastructure.
/// </summary>
public interface IAuditLogWriter
{
    Task WriteAsync(
        Guid? actorUserId,
        string action,
        string resourceType,
        string? resourceId,
        string? metadataJson = null,
        CancellationToken cancellationToken = default);
}

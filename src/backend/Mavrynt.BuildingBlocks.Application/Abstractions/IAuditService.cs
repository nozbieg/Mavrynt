namespace Mavrynt.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Records auditable events. The implementation lives in Infrastructure.
/// Folder structure is kept ready for extraction to a dedicated Audit module later.
/// </summary>
public interface IAuditService
{
    Task RecordAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}

using Mavrynt.BuildingBlocks.Domain.Primitives;

namespace Mavrynt.Modules.Audit.Domain.Entities;

/// <summary>
/// An immutable record of an administrative or system operation.
/// Append-only — no update or delete methods.
/// </summary>
public sealed class AuditLogEntry : Entity<Guid>
{
    private AuditLogEntry() : base(Guid.Empty) { }

    private AuditLogEntry(
        Guid id,
        Guid? actorUserId,
        string action,
        string resourceType,
        string? resourceId,
        DateTimeOffset occurredAt,
        string? metadataJson)
        : base(id)
    {
        ActorUserId = actorUserId;
        Action = action;
        ResourceType = resourceType;
        ResourceId = resourceId;
        OccurredAt = occurredAt;
        MetadataJson = metadataJson;
    }

    public Guid? ActorUserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string ResourceType { get; private set; } = string.Empty;
    public string? ResourceId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string? MetadataJson { get; private set; }

    public static AuditLogEntry Create(
        Guid? actorUserId,
        string action,
        string resourceType,
        string? resourceId,
        DateTimeOffset occurredAt,
        string? metadataJson = null)
    {
        return new AuditLogEntry(
            Guid.NewGuid(),
            actorUserId,
            action,
            resourceType,
            resourceId,
            occurredAt,
            metadataJson);
    }
}

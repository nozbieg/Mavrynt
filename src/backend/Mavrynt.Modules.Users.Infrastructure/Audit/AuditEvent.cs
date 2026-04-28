namespace Mavrynt.Modules.Users.Infrastructure.Audit;

/// <summary>
/// EF Core persistence model for an audit record.
/// Intentionally a plain class — no domain logic, no value objects.
/// Named and structured for later extraction to Mavrynt.Modules.Audit.Infrastructure.
/// </summary>
public sealed class AuditEvent
{
    // Private parameterless constructor for EF Core materialisation.
    private AuditEvent() { }

    public AuditEvent(
        Guid id,
        string eventType,
        DateTimeOffset occurredAt,
        Guid? userId = null,
        string? email = null,
        string? source = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? metadata = null)
    {
        Id = id;
        EventType = eventType;
        OccurredAt = occurredAt;
        UserId = userId;
        Email = email;
        Source = source;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Metadata = metadata;
    }

    public Guid Id { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public Guid? UserId { get; private set; }
    public string? Email { get; private set; }
    public string? Source { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    /// <summary>
    /// Free-form JSON string for additional context.
    /// Schema-free by design — structure is defined per event type by the emitter.
    /// </summary>
    public string? Metadata { get; private set; }
}

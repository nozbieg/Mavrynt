namespace Mavrynt.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Immutable payload describing a single auditable event.
/// Callers at the application layer populate what they know; HTTP-layer middleware
/// can enrich <see cref="IpAddress"/> and <see cref="UserAgent"/> before recording.
/// </summary>
public sealed record AuditEntry(
    string EventType,
    DateTimeOffset OccurredAt,
    Guid? UserId = null,
    string? Email = null,
    string? Source = null,
    string? IpAddress = null,
    string? UserAgent = null,
    string? Metadata = null
);

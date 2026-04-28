namespace Mavrynt.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Marker interface for commands and queries that should emit an audit entry.
/// Implement this on commands where security-relevant actions occur
/// (e.g. registration, login, email/password change).
///
/// The <see cref="AuditEventType"/> string is written to the audit log and should
/// uniquely describe the action (e.g. "Users.UserRegistered").
/// </summary>
public interface IAuditableRequest
{
    string AuditEventType { get; }
}

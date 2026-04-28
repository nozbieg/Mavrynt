namespace Mavrynt.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Marker interface for commands whose handler execution should be wrapped
/// in a database transaction via <see cref="Mavrynt.BuildingBlocks.Application.Persistence.IUnitOfWork"/>.
///
/// Rules:
/// - Only implement on mutating commands (write side).
/// - Do NOT implement on queries.
/// - The transaction behavior commits on handler success and
///   rolls back (or skips save) on failure or exception.
/// - If no IUnitOfWork is registered, the behavior is a safe no-op.
/// </summary>
public interface ITransactionalRequest;

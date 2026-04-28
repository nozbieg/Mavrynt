namespace Mavrynt.BuildingBlocks.Application.Persistence;

/// <summary>
/// Application-layer abstraction for committing a unit of work.
/// Implementations live in Infrastructure (e.g. EF Core DbContext).
///
/// Used by <see cref="Mavrynt.BuildingBlocks.Application.Behaviors.ITransactionalRequest"/>-aware
/// pipeline behaviors to save changes after a successful command handler execution.
///
/// If no implementation is registered in DI, the transaction behavior
/// acts as a safe no-op (changes are not committed via this mechanism).
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

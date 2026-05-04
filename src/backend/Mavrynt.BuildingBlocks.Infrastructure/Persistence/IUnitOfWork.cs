using Mavrynt.BuildingBlocks.Application.Persistence;

namespace Mavrynt.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Marker interface for Infrastructure-layer unit-of-work implementations
/// (e.g. EF Core <c>DbContext</c>). Extends the canonical Application-layer
/// <see cref="Application.Persistence.IUnitOfWork"/> contract.
///
/// IMPORTANT registration rule: the mediator pipeline
/// (<see cref="Mavrynt.BuildingBlocks.Application.Behaviors.TransactionBehavior{TRequest,TResponse}"/>)
/// resolves <see cref="Application.Persistence.IUnitOfWork"/>. DI does not
/// auto-resolve a base interface from a derived registration, so module
/// Infrastructure DI must register against the Application-layer type:
/// <code>
/// services.AddScoped&lt;Mavrynt.BuildingBlocks.Application.Persistence.IUnitOfWork&gt;(
///     sp =&gt; sp.GetRequiredService&lt;YourDbContext&gt;());
/// </code>
/// In a multi-module host, every module registers its own scoped DbContext as
/// <see cref="Application.Persistence.IUnitOfWork"/>; <c>TransactionBehavior</c>
/// resolves <c>IEnumerable&lt;IUnitOfWork&gt;</c> and commits each on handler
/// success (ADR-025).
/// </summary>
public interface IUnitOfWork : Application.Persistence.IUnitOfWork
{
}

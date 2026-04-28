using Mavrynt.BuildingBlocks.Application.Persistence;

namespace Mavrynt.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Infrastructure-layer unit of work. Extends the Application-layer
/// <see cref="Application.Persistence.IUnitOfWork"/> so that concrete
/// implementations (e.g. EF Core DbContext) satisfy the contract expected by
/// <see cref="Mavrynt.BuildingBlocks.Application.Behaviors.TransactionBehavior{TRequest,TResponse}"/>.
///
/// Register concrete implementations against both interfaces in Infrastructure DI:
///   services.AddScoped&lt;IUnitOfWork, YourDbContext&gt;();
/// (the Application IUnitOfWork is satisfied transitively via this interface)
/// </summary>
public interface IUnitOfWork : Application.Persistence.IUnitOfWork
{
}

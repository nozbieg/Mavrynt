using Mavrynt.BuildingBlocks.Domain.Abstractions;

namespace Mavrynt.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Generic repository abstraction for aggregate roots.
/// </summary>
public interface IRepository<TEntity, TId>
    where TEntity : IEntity<TId>
    where TId : notnull
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}

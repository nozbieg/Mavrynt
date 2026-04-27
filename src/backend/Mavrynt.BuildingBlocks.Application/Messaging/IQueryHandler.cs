using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.BuildingBlocks.Application.Messaging;

/// <summary>
/// Handler for a query that returns a <see cref="Result{TResponse}"/>.
/// </summary>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}

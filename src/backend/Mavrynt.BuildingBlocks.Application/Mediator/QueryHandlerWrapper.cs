using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.BuildingBlocks.Application.Mediator;

/// <summary>
/// Internal interface for dispatching a query that returns <see cref="Result{TResponse}"/>.
/// Cached per concrete query type to avoid repeated reflection.
/// </summary>
internal interface IQueryHandlerWrapper<TResponse>
{
    Task<Result<TResponse>> HandleAsync(IQuery<TResponse> query, IServiceProvider sp, CancellationToken ct);
}

/// <summary>
/// Closed-generic wrapper that resolves the typed handler and behavior pipeline
/// from DI and executes them for a specific <typeparamref name="TQuery"/> returning <typeparamref name="TResponse"/>.
/// </summary>
internal sealed class QueryHandlerWrapper<TQuery, TResponse> : IQueryHandlerWrapper<TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<Result<TResponse>> HandleAsync(IQuery<TResponse> query, IServiceProvider sp, CancellationToken ct)
    {
        var typedQuery = (TQuery)query;

        var handler = sp.GetService<IQueryHandler<TQuery, TResponse>>()
            ?? throw new InvalidOperationException(
                $"Handler '{typeof(IQueryHandler<TQuery, TResponse>).Name}' is not registered.");

        var behaviors = sp.GetServices<IMavryntBehavior<TQuery, Result<TResponse>>>().ToArray();

        Func<CancellationToken, Task<Result<TResponse>>> pipeline =
            token => handler.HandleAsync(typedQuery, token);

        for (var i = behaviors.Length - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var next = pipeline;
            pipeline = token => behavior.HandleAsync(typedQuery, next, token);
        }

        return await pipeline(ct);
    }
}

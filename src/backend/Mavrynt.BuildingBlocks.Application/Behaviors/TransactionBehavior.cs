using Mavrynt.BuildingBlocks.Application.Persistence;
using Mavrynt.BuildingBlocks.Domain.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mavrynt.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Pipeline behavior that wraps the handler execution in a unit-of-work commit
/// for commands implementing <see cref="ITransactionalRequest"/>.
///
/// Rules:
///   - Activates only when the request implements <see cref="ITransactionalRequest"/>.
///   - Resolves every <see cref="IUnitOfWork"/> registered in DI and commits each on
///     handler success. In a multi-module host this means each module's DbContext
///     flushes its own change tracker. EF Core only emits SQL when there are pending
///     changes, so contexts with no work to commit are essentially free.
///   - Calls <see cref="IUnitOfWork.SaveChangesAsync"/> only on handler success.
///   - Does NOT commit on failure result or exception (EF Core rolls back on dispose).
///   - Queries must NOT implement <see cref="ITransactionalRequest"/>.
///   - Does not open an explicit cross-context DB transaction — each SaveChanges runs
///     in its own EF Core implicit transaction. Cross-context atomicity is out of scope
///     for now (see ADR-025).
///
/// Pipeline order: FIFTH / LAST (wraps the handler call directly).
/// </summary>
public sealed class TransactionBehavior<TRequest, TResponse> : IMavryntBehavior<TRequest, TResponse>
    where TResponse : notnull
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IServiceProvider serviceProvider,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        Func<CancellationToken, Task<TResponse>> next,
        CancellationToken cancellationToken)
    {
        if (request is not ITransactionalRequest)
            return await next(cancellationToken);

        var unitsOfWork = _serviceProvider.GetServices<IUnitOfWork>().ToList();
        if (unitsOfWork.Count == 0)
        {
            _logger.LogDebug(
                "Request {RequestType} is transactional but no IUnitOfWork is registered. Executing without commit.",
                typeof(TRequest).Name);

            return await next(cancellationToken);
        }

        var response = await next(cancellationToken);

        // Commit only on success — do not persist changes when the handler signals failure.
        if (IsSuccess(response))
        {
            foreach (var unitOfWork in unitsOfWork)
                await unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogDebug(
                "Transaction committed for {RequestType} across {UnitOfWorkCount} unit(s) of work.",
                typeof(TRequest).Name,
                unitsOfWork.Count);
        }
        else
        {
            _logger.LogDebug(
                "Transaction NOT committed for {RequestType} — handler returned failure.",
                typeof(TRequest).Name);
        }

        return response;
    }

    private static bool IsSuccess(TResponse response) =>
        response is not Result r || r.IsSuccess;
}

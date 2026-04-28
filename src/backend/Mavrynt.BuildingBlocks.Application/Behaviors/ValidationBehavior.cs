using Mavrynt.BuildingBlocks.Application.Validation;
using Mavrynt.BuildingBlocks.Domain.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Pipeline behavior that runs all registered <see cref="IValidator{TRequest}"/> instances
/// for the current request before invoking the handler.
///
/// Rules:
///   - All <see cref="IValidator{TRequest}"/> implementations for <typeparamref name="TRequest"/>
///     are resolved from DI and executed in registration order.
///   - Stops on the first validation failure and returns <see cref="Result"/> failure
///     without calling the handler.
///   - If no validators are registered, the behavior is a transparent no-op.
///   - Validation exceptions are re-thrown; the mediator's outer catch converts them to errors.
///
/// Pipeline order: SECOND (after Logging, before Resilience / Audit / Transaction).
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IMavryntBehavior<TRequest, TResponse>
    where TResponse : notnull
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        Func<CancellationToken, Task<TResponse>> next,
        CancellationToken cancellationToken)
    {
        var validators = _serviceProvider
            .GetServices<IValidator<TRequest>>()
            .ToArray();

        if (validators.Length == 0)
            return await next(cancellationToken);

        foreach (var validator in validators)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult.IsFailure)
            {
                // Return early — do not call handler or subsequent behaviors.
                return CreateFailureResponse(validationResult.Error);
            }
        }

        return await next(cancellationToken);
    }

    private static TResponse CreateFailureResponse(Error error)
    {
        // TResponse is either Result or Result<T>. Both expose a static Failure factory.
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(error);

        // Result<T>: use Result.Failure<T>(error) via the generic static method.
        var resultType = typeof(Result);
        var method = resultType.GetMethod(nameof(Result.Failure), 1, [typeof(Error)])!
            .MakeGenericMethod(typeof(TResponse).GetGenericArguments()[0]);

        return (TResponse)method.Invoke(null, [error])!;
    }
}

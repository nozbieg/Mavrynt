using Microsoft.Extensions.Logging;

namespace Mavrynt.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Pipeline behavior that provides a hook for retry / timeout / circuit-breaker logic.
///
/// Current state:
///   - Pass-through for all requests.
///   - Logs when a request is marked <see cref="IResilientRequest"/> for observability.
///   - No Polly dependency is introduced at this stage.
///
/// Future extension:
///   - Add Polly when an ADR approves the dependency.
///   - Implement retry based on <see cref="IResilientRequest.MaxRetryAttempts"/>.
///   - Implement timeout based on <see cref="IResilientRequest.TimeoutMs"/>.
///
/// Safety:
///   - Non-idempotent commands (Register, Login, ChangePassword) must NOT implement
///     <see cref="IResilientRequest"/> — retrying them blindly causes duplicate side effects.
///
/// Pipeline order: THIRD (after Logging, Validation; before Audit, Transaction).
/// </summary>
public sealed class ResilienceBehavior<TRequest, TResponse> : IMavryntBehavior<TRequest, TResponse>
    where TResponse : notnull
{
    private readonly ILogger<ResilienceBehavior<TRequest, TResponse>> _logger;

    public ResilienceBehavior(ILogger<ResilienceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        Func<CancellationToken, Task<TResponse>> next,
        CancellationToken cancellationToken)
    {
        if (request is IResilientRequest resilient)
        {
            _logger.LogDebug(
                "Request {RequestType} is marked resilient. MaxRetry: {MaxRetry}, TimeoutMs: {TimeoutMs}. " +
                "Polly not yet wired — executing without retry policy.",
                typeof(TRequest).Name, resilient.MaxRetryAttempts, resilient.TimeoutMs);
        }

        return await next(cancellationToken);
    }
}

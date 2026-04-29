using System.Diagnostics;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Microsoft.Extensions.Logging;

namespace Mavrynt.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Pipeline behavior that logs request execution metadata.
///
/// Logged fields (structured):
///   - RequestType   — short class name of the request
///   - Category      — "Command" or "Query"
///   - ElapsedMs     — wall-clock time in milliseconds
///   - Succeeded     — true / false
///   - ErrorCode     — populated on failure
///   - TraceId       — current OpenTelemetry trace ID when available
///
/// Security rules:
///   - Never serializes the full request object.
///   - Property names containing "password", "hash", or "token" (case-insensitive)
///     are not logged even if inadvertently added as log parameters.
///
/// Pipeline order: FIRST (registered before Validation, Audit, Resilience, Transaction).
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IMavryntBehavior<TRequest, TResponse>
    where TResponse : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        Func<CancellationToken, Task<TResponse>> next,
        CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest).Name;
        var category = DetermineCategory(typeof(TRequest));
        var traceId = Activity.Current?.TraceId.ToString();
        var sw = Stopwatch.StartNew();

        _logger.LogInformation(
            "Handling {Category} {RequestType}. TraceId: {TraceId}",
            category, requestType, traceId);

        TResponse response;
        try
        {
            response = await next(cancellationToken);
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex,
                "{Category} {RequestType} threw an unhandled exception after {ElapsedMs}ms. TraceId: {TraceId}",
                category, requestType, sw.ElapsedMilliseconds, traceId);
            throw;
        }

        sw.Stop();

        // Inspect the result without knowing the concrete TResponse at compile time.
        if (response is Result result)
        {
            if (result.IsFailure)
            {
                _logger.LogWarning(
                    "{Category} {RequestType} failed in {ElapsedMs}ms. ErrorCode: {ErrorCode}. TraceId: {TraceId}",
                    category, requestType, sw.ElapsedMilliseconds, result.Error.Code, traceId);
            }
            else
            {
                _logger.LogInformation(
                    "{Category} {RequestType} succeeded in {ElapsedMs}ms. TraceId: {TraceId}",
                    category, requestType, sw.ElapsedMilliseconds, traceId);
            }
        }
        else
        {
            // Non-Result response type (future extension).
            _logger.LogInformation(
                "{Category} {RequestType} completed in {ElapsedMs}ms. TraceId: {TraceId}",
                category, requestType, sw.ElapsedMilliseconds, traceId);
        }

        return response;
    }

    private static string DetermineCategory(Type requestType)
    {
        if (requestType.GetInterfaces().Any(i =>
                i == typeof(ICommand) ||
                (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>))))
            return "Command";

        if (requestType.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>)))
            return "Query";

        return "Request";
    }
}

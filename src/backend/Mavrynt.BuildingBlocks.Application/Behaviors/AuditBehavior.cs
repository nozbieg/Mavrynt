using System.Diagnostics;
using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Domain.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mavrynt.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Pipeline behavior that writes an audit entry for requests implementing <see cref="IAuditableRequest"/>.
///
/// Rules:
///   - Only activates when the request implements <see cref="IAuditableRequest"/>.
///   - Writes audit after the handler completes (success or failure).
///   - Includes: event type, occurred-at UTC, success/failure, error code, trace ID.
///   - Never includes secrets (password, token, hash).
///   - Uses <see cref="IAuditService"/> resolved optionally from DI.
///     If not registered, the behavior is a safe no-op.
///   - Uses <see cref="ICurrentUserContext"/> (optional) for UserId.
///
/// Pipeline order: FOURTH (after Logging, Validation, Resilience; before Transaction).
/// </summary>
public sealed class AuditBehavior<TRequest, TResponse> : IMavryntBehavior<TRequest, TResponse>
    where TResponse : notnull
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditBehavior<TRequest, TResponse>> _logger;

    public AuditBehavior(
        IServiceProvider serviceProvider,
        ILogger<AuditBehavior<TRequest, TResponse>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        Func<CancellationToken, Task<TResponse>> next,
        CancellationToken cancellationToken)
    {
        if (request is not IAuditableRequest auditable)
            return await next(cancellationToken);

        var auditService = _serviceProvider.GetService<IAuditService>();
        if (auditService is null)
            return await next(cancellationToken);

        var response = await next(cancellationToken);

        try
        {
            var currentUser = _serviceProvider.GetService<ICurrentUserContext>();
            var traceId = Activity.Current?.TraceId.ToString();
            var isSuccess = IsSuccess(response);
            var errorCode = isSuccess ? null : GetErrorCode(response);

            var entry = new AuditEntry(
                EventType: auditable.AuditEventType,
                OccurredAt: DateTimeOffset.UtcNow,
                UserId: currentUser?.UserId,
                Metadata: traceId is not null
                    ? $"{{\"traceId\":\"{traceId}\",\"success\":{isSuccess.ToString().ToLowerInvariant()},\"errorCode\":\"{errorCode}\"}}"
                    : $"{{\"success\":{isSuccess.ToString().ToLowerInvariant()},\"errorCode\":\"{errorCode}\"}}"
            );

            await auditService.RecordAsync(entry, cancellationToken);
        }
        catch (Exception ex)
        {
            // Audit failure must never break the main request flow.
            _logger.LogWarning(ex, "Audit write failed for {EventType} on {RequestType}",
                auditable.AuditEventType, typeof(TRequest).Name);
        }

        return response;
    }

    private static bool IsSuccess(TResponse response) =>
        response is Result r && r.IsSuccess;

    private static string? GetErrorCode(TResponse response) =>
        response is Result r && r.IsFailure ? r.Error.Code : null;
}

using System.Collections.Concurrent;
using System.Diagnostics;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Microsoft.Extensions.Logging;

namespace Mavrynt.BuildingBlocks.Application.Mediator;

/// <summary>
/// Mavrynt's internal mediator implementation. Do not use directly — depend on <see cref="IMediator"/>.
///
/// Dispatch flow:
///   1. Resolve a cached wrapper for the concrete request type (one reflection call per type lifetime).
///   2. The wrapper resolves the handler + all registered <c>IMavryntBehavior&lt;TRequest, TResponse&gt;</c>
///      from DI and builds the pipeline in registration order.
///   3. Unhandled exceptions are caught here, logged, and returned as <see cref="Result"/> failures.
///      The trace ID is attached to the error when an active OpenTelemetry activity exists.
/// </summary>
internal sealed class MavryntMediator : IMediator
{
    // Separate caches per dispatch path — each path has a distinct wrapper interface.
    private static readonly ConcurrentDictionary<Type, ICommandHandlerWrapper> CommandWrappers = new();
    private static readonly ConcurrentDictionary<Type, object> CommandWithResponseWrappers = new();
    private static readonly ConcurrentDictionary<Type, object> QueryWrappers = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MavryntMediator> _logger;

    public MavryntMediator(IServiceProvider serviceProvider, ILogger<MavryntMediator> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    // ── ICommand (returns Result) ─────────────────────────────────────────────

    public async Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        var requestType = command.GetType();

        try
        {
            var wrapper = CommandWrappers.GetOrAdd(requestType, static t =>
            {
                var wrapperType = typeof(CommandHandlerWrapper<>).MakeGenericType(t);
                return (ICommandHandlerWrapper)Activator.CreateInstance(wrapperType)!;
            });

            return await wrapper.HandleAsync(command, _serviceProvider, cancellationToken);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("is not registered"))
        {
            _logger.LogError(ex, "Handler not found for command {CommandType}", requestType.Name);
            return Result.Failure(MediatorErrors.HandlerNotFound(requestType.Name));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogError(ex, "Unhandled exception in mediator for command {CommandType}. TraceId: {TraceId}",
                requestType.Name, traceId);
            return Result.Failure(MediatorErrors.UnhandledException(requestType.Name, traceId));
        }
    }

    // ── ICommand<TResponse> (returns Result<TResponse>) ───────────────────────

    public async Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var requestType = command.GetType();

        try
        {
            var wrapper = (ICommandWithResponseHandlerWrapper<TResponse>)CommandWithResponseWrappers.GetOrAdd(
                requestType,
                static (t, responseType) =>
                {
                    var wrapperType = typeof(CommandWithResponseHandlerWrapper<,>).MakeGenericType(t, responseType);
                    return Activator.CreateInstance(wrapperType)!;
                },
                typeof(TResponse));

            return await wrapper.HandleAsync(command, _serviceProvider, cancellationToken);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("is not registered"))
        {
            _logger.LogError(ex, "Handler not found for command {CommandType}", requestType.Name);
            return Result.Failure<TResponse>(MediatorErrors.HandlerNotFound(requestType.Name));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogError(ex, "Unhandled exception in mediator for command {CommandType}. TraceId: {TraceId}",
                requestType.Name, traceId);
            return Result.Failure<TResponse>(MediatorErrors.UnhandledException(requestType.Name, traceId));
        }
    }

    // ── IQuery<TResponse> (returns Result<TResponse>) ─────────────────────────

    public async Task<Result<TResponse>> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var requestType = query.GetType();

        try
        {
            var wrapper = (IQueryHandlerWrapper<TResponse>)QueryWrappers.GetOrAdd(
                requestType,
                static (t, responseType) =>
                {
                    var wrapperType = typeof(QueryHandlerWrapper<,>).MakeGenericType(t, responseType);
                    return Activator.CreateInstance(wrapperType)!;
                },
                typeof(TResponse));

            return await wrapper.HandleAsync(query, _serviceProvider, cancellationToken);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("is not registered"))
        {
            _logger.LogError(ex, "Handler not found for query {QueryType}", requestType.Name);
            return Result.Failure<TResponse>(MediatorErrors.HandlerNotFound(requestType.Name));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogError(ex, "Unhandled exception in mediator for query {QueryType}. TraceId: {TraceId}",
                requestType.Name, traceId);
            return Result.Failure<TResponse>(MediatorErrors.UnhandledException(requestType.Name, traceId));
        }
    }
}

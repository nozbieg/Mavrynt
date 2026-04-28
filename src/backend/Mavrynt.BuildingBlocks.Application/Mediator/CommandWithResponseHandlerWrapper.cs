using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.BuildingBlocks.Application.Mediator;

/// <summary>
/// Internal interface for dispatching a command that returns <see cref="Result{TResponse}"/>.
/// Cached per concrete command type to avoid repeated reflection.
/// </summary>
internal interface ICommandWithResponseHandlerWrapper<TResponse>
{
    Task<Result<TResponse>> HandleAsync(IBaseCommand command, IServiceProvider sp, CancellationToken ct);
}

/// <summary>
/// Closed-generic wrapper that resolves the typed handler and behavior pipeline
/// from DI and executes them for a specific <typeparamref name="TCommand"/> returning <typeparamref name="TResponse"/>.
/// </summary>
internal sealed class CommandWithResponseHandlerWrapper<TCommand, TResponse> : ICommandWithResponseHandlerWrapper<TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> HandleAsync(IBaseCommand command, IServiceProvider sp, CancellationToken ct)
    {
        var typedCommand = (TCommand)command;

        var handler = sp.GetService<ICommandHandler<TCommand, TResponse>>()
            ?? throw new InvalidOperationException(
                $"Handler '{typeof(ICommandHandler<TCommand, TResponse>).Name}' is not registered.");

        var behaviors = sp.GetServices<IMavryntBehavior<TCommand, Result<TResponse>>>().ToArray();

        Func<CancellationToken, Task<Result<TResponse>>> pipeline =
            token => handler.HandleAsync(typedCommand, token);

        for (var i = behaviors.Length - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var next = pipeline;
            pipeline = token => behavior.HandleAsync(typedCommand, next, token);
        }

        return await pipeline(ct);
    }
}

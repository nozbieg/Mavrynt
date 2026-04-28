
using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.BuildingBlocks.Application.Mediator;

/// <summary>
/// Internal interface for dispatching a void command (returns <see cref="Result"/>).
/// Cached per concrete command type to avoid repeated reflection.
/// </summary>
internal interface ICommandHandlerWrapper
{
    Task<Result> HandleAsync(ICommand command, IServiceProvider sp, CancellationToken ct);
}

/// <summary>
/// Closed-generic wrapper that resolves the typed handler and behavior pipeline
/// from DI and executes them for a specific <typeparamref name="TCommand"/>.
/// </summary>
internal sealed class CommandHandlerWrapper<TCommand> : ICommandHandlerWrapper
    where TCommand : ICommand
{
    public async Task<Result> HandleAsync(ICommand command, IServiceProvider sp, CancellationToken ct)
    {
        var typedCommand = (TCommand)command;

        var handler = sp.GetService<ICommandHandler<TCommand>>()
            ?? throw new InvalidOperationException(
                $"Handler '{typeof(ICommandHandler<TCommand>).Name}' is not registered.");

        var behaviors = sp.GetServices<IMavryntBehavior<TCommand, Result>>().ToArray();

        Func<CancellationToken, Task<Result>> pipeline =
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

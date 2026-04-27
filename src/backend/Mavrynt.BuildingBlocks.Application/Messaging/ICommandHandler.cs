using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.BuildingBlocks.Application.Messaging;

/// <summary>
/// Handler for a command that returns a plain <see cref="Result"/>.
/// </summary>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handler for a command that returns a <see cref="Result{TResponse}"/>.
/// </summary>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

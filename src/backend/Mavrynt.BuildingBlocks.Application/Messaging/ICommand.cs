using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.BuildingBlocks.Application.Messaging;

/// <summary>
/// Marker interface for a command that returns a plain <see cref="Result"/>.
/// </summary>
public interface ICommand : IBaseCommand;

/// <summary>
/// Marker interface for a command that returns a <see cref="Result{TResponse}"/>.
/// </summary>
public interface ICommand<TResponse> : IBaseCommand;

/// <summary>
/// Common marker for all commands.
/// </summary>
public interface IBaseCommand;

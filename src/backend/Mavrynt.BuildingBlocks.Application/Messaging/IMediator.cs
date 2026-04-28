using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.BuildingBlocks.Application.Messaging;

/// <summary>
/// Internal mediator. Dispatches commands and queries through the behavior pipeline
/// and resolves the matching handler from DI.
///
/// Usage in endpoints and application services:
///   var result = await mediator.SendAsync(new RegisterUserCommand(...), ct);
///
/// Rules for AI agents:
/// - Endpoints must inject <see cref="IMediator"/>, not concrete handler interfaces.
/// - Do not bypass the mediator from hosts.
/// - Do not add MediatR — this is Mavrynt's own implementation.
/// </summary>
public interface IMediator
{
    /// <summary>Dispatches a command that returns a plain <see cref="Result"/>.</summary>
    Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default);

    /// <summary>Dispatches a command that returns a typed <see cref="Result{TResponse}"/>.</summary>
    Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

    /// <summary>Dispatches a query that returns a typed <see cref="Result{TResponse}"/>.</summary>
    Task<Result<TResponse>> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}

using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.BuildingBlocks.Application.Messaging;

/// <summary>
/// Marker interface for a query that returns a <see cref="Result{TResponse}"/>.
/// </summary>
public interface IQuery<TResponse>;

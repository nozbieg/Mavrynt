using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.BuildingBlocks.Application.Mediator;

/// <summary>
/// Well-known errors produced by the mediator infrastructure itself.
/// These are distinct from domain errors and represent pipeline-level failures.
/// </summary>
internal static class MediatorErrors
{
    public static Error HandlerNotFound(string requestTypeName) =>
        new("Mediator.HandlerNotFound",
            $"No handler registered for request type '{requestTypeName}'. " +
            "Ensure the handler assembly is passed to AddMavryntMediator.");

    public static Error UnhandledException(string requestTypeName, string? traceId) =>
        new("Mediator.UnhandledException",
            $"An unhandled exception occurred while processing '{requestTypeName}'." +
            (traceId is not null ? $" TraceId: {traceId}" : string.Empty));
}

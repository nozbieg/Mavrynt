namespace Mavrynt.BuildingBlocksContracts.Events;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTimeOffset OccurredOn { get; }
    string? CorrelationId { get; }
}

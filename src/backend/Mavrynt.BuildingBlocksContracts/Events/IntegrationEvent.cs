namespace Mavrynt.BuildingBlocksContracts.Events;

public abstract class IntegrationEvent : IIntegrationEvent
{
    protected IntegrationEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTimeOffset.UtcNow;
    }

    protected IntegrationEvent(Guid id, DateTimeOffset occurredOn, string? correlationId = null)
    {
        Id = id;
        OccurredOn = occurredOn;
        CorrelationId = correlationId;
    }

    public Guid Id { get; }
    public DateTimeOffset OccurredOn { get; }
    public string? CorrelationId { get; }
}

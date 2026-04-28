using Mavrynt.BuildingBlocks.Domain.Abstractions;
using Mavrynt.BuildingBlocks.Domain.Primitives;

namespace Mavrynt.BuildingBlocks.Domain.Tests;

public sealed class PrimitivesTests
{
    [Fact]
    public void Entity_Should_Be_Equal_By_Identity()
    {
        var id = Guid.NewGuid();
        Assert.Equal(new SampleEntity(id), new SampleEntity(id));
    }

    [Fact]
    public void ValueObject_Should_Be_Equal_By_Components()
    {
        Assert.Equal(new SampleValueObject("same", 1), new SampleValueObject("same", 1));
    }

    [Fact]
    public void Aggregate_Should_Collect_And_Clear_Domain_Events()
    {
        var aggregate = new SampleAggregate(Guid.NewGuid());
        aggregate.Raise();

        Assert.Single(aggregate.DomainEvents);

        aggregate.ClearDomainEvents();
        Assert.Empty(aggregate.DomainEvents);
    }

    private sealed class SampleEntity(Guid id) : Entity<Guid>(id);

    private sealed class SampleValueObject(string text, int number) : ValueObject
    {
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return text;
            yield return number;
        }
    }

    private sealed class SampleAggregate(Guid id) : AggregateRoot<Guid>(id)
    {
        public void Raise() => RaiseDomainEvent(new SampleEvent(Guid.NewGuid(), DateTimeOffset.UtcNow));
    }

    private sealed record SampleEvent(Guid Id, DateTimeOffset OccurredOn) : IDomainEvent;
}

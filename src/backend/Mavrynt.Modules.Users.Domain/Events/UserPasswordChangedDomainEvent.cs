using Mavrynt.BuildingBlocks.Domain.Abstractions;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Domain.Events;

public sealed record UserPasswordChangedDomainEvent(
    Guid Id,
    DateTimeOffset OccurredOn,
    UserId UserId
) : IDomainEvent;

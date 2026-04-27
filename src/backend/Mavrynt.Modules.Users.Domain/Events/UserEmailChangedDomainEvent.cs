using Mavrynt.BuildingBlocks.Domain.Abstractions;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Domain.Events;

public sealed record UserEmailChangedDomainEvent(
    Guid Id,
    DateTimeOffset OccurredOn,
    UserId UserId,
    Email OldEmail,
    Email NewEmail
) : IDomainEvent;

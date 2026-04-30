using Mavrynt.BuildingBlocks.Application.Abstractions;

namespace Mavrynt.Modules.Notifications.Infrastructure.Time;

internal sealed class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

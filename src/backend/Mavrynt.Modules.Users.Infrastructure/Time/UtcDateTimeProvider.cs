using Mavrynt.BuildingBlocks.Application.Abstractions;

namespace Mavrynt.Modules.Users.Infrastructure.Time;

/// <summary>
/// Returns the current UTC time. Registered as a singleton.
/// </summary>
internal sealed class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

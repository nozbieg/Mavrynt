using Mavrynt.BuildingBlocks.Application.Abstractions;

namespace Mavrynt.Modules.FeatureManagement.Infrastructure.Time;

internal sealed class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

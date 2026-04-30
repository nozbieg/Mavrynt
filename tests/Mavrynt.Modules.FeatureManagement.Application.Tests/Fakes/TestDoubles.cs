using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.Modules.Audit.Application.Abstractions;
using Mavrynt.Modules.FeatureManagement.Domain.Entities;
using Mavrynt.Modules.FeatureManagement.Domain.Repositories;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;

namespace Mavrynt.Modules.FeatureManagement.Application.Tests.Fakes;

internal sealed class FixedDateTimeProvider(DateTimeOffset utcNow) : IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; } = utcNow;
}

internal sealed class FakeAuditLogWriter : IAuditLogWriter
{
    public List<(Guid? ActorUserId, string Action, string ResourceType, string? ResourceId)> Entries { get; } = [];

    public Task WriteAsync(
        Guid? actorUserId,
        string action,
        string resourceType,
        string? resourceId,
        string? metadataJson = null,
        CancellationToken cancellationToken = default)
    {
        Entries.Add((actorUserId, action, resourceType, resourceId));
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryFeatureFlagRepository : IFeatureFlagRepository
{
    public List<FeatureFlag> Flags { get; } = [];

    public void Seed(FeatureFlag flag) => Flags.Add(flag);

    public Task<FeatureFlag?> GetByIdAsync(FeatureFlagId id, CancellationToken cancellationToken = default)
        => Task.FromResult(Flags.FirstOrDefault(f => f.Id == id));

    public Task<FeatureFlag?> GetByKeyAsync(FeatureFlagKey key, CancellationToken cancellationToken = default)
        => Task.FromResult(Flags.FirstOrDefault(f => f.Key == key));

    public Task<IReadOnlyList<FeatureFlag>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<FeatureFlag>>(Flags.AsReadOnly());

    public Task<bool> ExistsByKeyAsync(FeatureFlagKey key, CancellationToken cancellationToken = default)
        => Task.FromResult(Flags.Any(f => f.Key == key));

    public Task AddAsync(FeatureFlag featureFlag, CancellationToken cancellationToken = default)
    {
        Flags.Add(featureFlag);
        return Task.CompletedTask;
    }
}

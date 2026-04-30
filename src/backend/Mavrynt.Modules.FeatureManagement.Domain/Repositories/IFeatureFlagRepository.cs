using Mavrynt.Modules.FeatureManagement.Domain.Entities;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;

namespace Mavrynt.Modules.FeatureManagement.Domain.Repositories;

public interface IFeatureFlagRepository
{
    Task<FeatureFlag?> GetByIdAsync(FeatureFlagId id, CancellationToken cancellationToken = default);
    Task<FeatureFlag?> GetByKeyAsync(FeatureFlagKey key, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeatureFlag>> ListAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByKeyAsync(FeatureFlagKey key, CancellationToken cancellationToken = default);
    Task AddAsync(FeatureFlag featureFlag, CancellationToken cancellationToken = default);
}

using Mavrynt.Modules.FeatureManagement.Domain.Entities;
using Mavrynt.Modules.FeatureManagement.Domain.Repositories;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;
using Mavrynt.Modules.FeatureManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mavrynt.Modules.FeatureManagement.Infrastructure.Repositories;

internal sealed class FeatureFlagRepository : IFeatureFlagRepository
{
    private readonly FeatureManagementDbContext _context;

    public FeatureFlagRepository(FeatureManagementDbContext context)
    {
        _context = context;
    }

    public Task<FeatureFlag?> GetByIdAsync(FeatureFlagId id, CancellationToken cancellationToken = default)
        => _context.FeatureFlags.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public Task<FeatureFlag?> GetByKeyAsync(FeatureFlagKey key, CancellationToken cancellationToken = default)
        => _context.FeatureFlags.FirstOrDefaultAsync(f => f.Key == key, cancellationToken);

    public async Task<IReadOnlyList<FeatureFlag>> ListAsync(CancellationToken cancellationToken = default)
        => await _context.FeatureFlags.OrderBy(f => f.Key).ToListAsync(cancellationToken);

    public Task<bool> ExistsByKeyAsync(FeatureFlagKey key, CancellationToken cancellationToken = default)
        => _context.FeatureFlags.AnyAsync(f => f.Key == key, cancellationToken);

    public async Task AddAsync(FeatureFlag featureFlag, CancellationToken cancellationToken = default)
    {
        await _context.FeatureFlags.AddAsync(featureFlag, cancellationToken);
    }
}

using Mavrynt.BuildingBlocks.Infrastructure.Persistence;
using Mavrynt.Modules.FeatureManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mavrynt.Modules.FeatureManagement.Infrastructure.Persistence;

public sealed class FeatureManagementDbContext : DbContext, IUnitOfWork
{
    public FeatureManagementDbContext(DbContextOptions<FeatureManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FeatureManagementDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

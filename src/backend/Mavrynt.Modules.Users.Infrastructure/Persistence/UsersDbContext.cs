using Mavrynt.BuildingBlocks.Infrastructure.Persistence;
using Mavrynt.Modules.Users.Domain.Entities;
using Mavrynt.Modules.Users.Infrastructure.Audit;
using Microsoft.EntityFrameworkCore;

namespace Mavrynt.Modules.Users.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for the Users module.
/// Implements <see cref="IUnitOfWork"/> so the same instance can be injected
/// when callers need to commit changes explicitly.
/// </summary>
public sealed class UsersDbContext : DbContext, IUnitOfWork
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

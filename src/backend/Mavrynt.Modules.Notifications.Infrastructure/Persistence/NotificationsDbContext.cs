using Mavrynt.BuildingBlocks.Infrastructure.Persistence;
using Mavrynt.Modules.Notifications.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mavrynt.Modules.Notifications.Infrastructure.Persistence;

public sealed class NotificationsDbContext : DbContext, IUnitOfWork
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options) { }

    public DbSet<SmtpSettings> SmtpSettings => Set<SmtpSettings>();
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

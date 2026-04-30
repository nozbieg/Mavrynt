using Mavrynt.Modules.Audit.Application.Abstractions;
using Mavrynt.Modules.Audit.Infrastructure.Persistence;
using Mavrynt.Modules.Audit.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.Modules.Audit.Infrastructure.DependencyInjection;

public static class AuditInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddAuditInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AuditDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("MavryntDb")
                ?? throw new InvalidOperationException(
                    "Connection string 'MavryntDb' is not configured.");

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "audit");
            });
        });

        services.AddScoped<IAuditLogWriter, EfAuditLogWriter>();
        services.AddHostedService<DatabaseMigrationService>();

        return services;
    }
}

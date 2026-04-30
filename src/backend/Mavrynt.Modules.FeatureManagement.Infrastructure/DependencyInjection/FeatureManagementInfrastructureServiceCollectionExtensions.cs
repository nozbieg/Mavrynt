using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Infrastructure.Persistence;
using Mavrynt.Modules.FeatureManagement.Domain.Repositories;
using Mavrynt.Modules.FeatureManagement.Infrastructure.Persistence;
using Mavrynt.Modules.FeatureManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mavrynt.Modules.FeatureManagement.Infrastructure.DependencyInjection;

public static class FeatureManagementInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddFeatureManagementInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FeatureManagementDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("MavryntDb")
                ?? throw new InvalidOperationException(
                    "Connection string 'MavryntDb' is not configured.");

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "feature_management");
            });
        });

        services.AddScoped<IFeatureFlagRepository, FeatureFlagRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<FeatureManagementDbContext>());
        services.AddHostedService<DatabaseMigrationService>();

        services.TryAddSingleton<IDateTimeProvider, Mavrynt.Modules.FeatureManagement.Infrastructure.Time.UtcDateTimeProvider>();

        return services;
    }
}

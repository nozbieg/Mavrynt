using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mavrynt.Modules.FeatureManagement.Infrastructure.Persistence;

internal sealed class DatabaseMigrationService(
    IServiceProvider serviceProvider,
    ILogger<DatabaseMigrationService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Applying FeatureManagement module database migrations…");

        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<FeatureManagementDbContext>();

        await context.Database.ExecuteSqlRawAsync(
            """
            CREATE SCHEMA IF NOT EXISTS feature_management;
            CREATE TABLE IF NOT EXISTS feature_management.__ef_migrations_history (
                "MigrationId" character varying(150) NOT NULL,
                "ProductVersion" character varying(32) NOT NULL,
                CONSTRAINT "PK___ef_migrations_history" PRIMARY KEY ("MigrationId")
            );
            """,
            Array.Empty<object>(),
            cancellationToken);

        await context.Database.MigrateAsync(cancellationToken);

        logger.LogInformation("FeatureManagement module database migrations applied successfully.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

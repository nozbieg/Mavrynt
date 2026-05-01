using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mavrynt.Modules.Audit.Infrastructure.Persistence;

internal sealed class DatabaseMigrationService(
    IServiceProvider serviceProvider,
    ILogger<DatabaseMigrationService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Applying Audit module database migrations…");

        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AuditDbContext>();

        // EF Core queries __ef_migrations_history before running any migration.
        // On a fresh database the "audit" schema doesn't exist yet, so PostgreSQL
        // throws "schema does not exist" and EF Core logs a misleading `fail:` entry.
        // Pre-creating the schema and the history table eliminates the noisy failure:
        // MigrateAsync then reads an empty history table and proceeds cleanly.
        await context.Database.ExecuteSqlRawAsync(
            """
            CREATE SCHEMA IF NOT EXISTS audit;
            CREATE TABLE IF NOT EXISTS audit.__ef_migrations_history (
                "MigrationId" character varying(150) NOT NULL,
                "ProductVersion" character varying(32) NOT NULL,
                CONSTRAINT "PK___ef_migrations_history" PRIMARY KEY ("MigrationId")
            );
            """,
            Array.Empty<object>(),
            cancellationToken);

        await context.Database.MigrateAsync(cancellationToken);

        logger.LogInformation("Audit module database migrations applied successfully.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

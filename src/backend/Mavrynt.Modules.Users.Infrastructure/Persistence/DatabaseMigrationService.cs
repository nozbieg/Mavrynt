using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mavrynt.Modules.Users.Infrastructure.Persistence;

/// <summary>
/// Applies any pending EF Core migrations for the Users module on application startup.
/// Runs once before the host begins accepting requests.
/// </summary>
internal sealed class DatabaseMigrationService(
    IServiceProvider serviceProvider,
    ILogger<DatabaseMigrationService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Applying Users module database migrations…");

        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        // MigrateAsync is idempotent: creates the schema/history table on first run,
        // applies any pending migrations, and no-ops when the schema is current.
        // PostgreSQL advisory locking inside MigrateAsync makes concurrent startup safe.
        await context.Database.MigrateAsync(cancellationToken);

        logger.LogInformation("Users module database migrations applied successfully.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

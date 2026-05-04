using Mavrynt.Modules.Notifications.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mavrynt.Modules.Notifications.Infrastructure.Persistence;

internal sealed class NotificationsStartupService(
    IServiceProvider serviceProvider,
    ILogger<NotificationsStartupService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Applying Notifications module database migrations…");

        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
        var templateSeeder = scope.ServiceProvider.GetRequiredService<DefaultEmailTemplateSeeder>();
        var smtpSeeder = scope.ServiceProvider.GetRequiredService<DefaultSmtpSettingsSeeder>();

        await context.Database.ExecuteSqlRawAsync(
            """
            CREATE SCHEMA IF NOT EXISTS notifications;
            CREATE TABLE IF NOT EXISTS notifications.__ef_migrations_history (
                "MigrationId" character varying(150) NOT NULL,
                "ProductVersion" character varying(32) NOT NULL,
                CONSTRAINT "PK___ef_migrations_history" PRIMARY KEY ("MigrationId")
            );
            """,
            Array.Empty<object>(),
            cancellationToken);

        await context.Database.MigrateAsync(cancellationToken);
        logger.LogInformation("Notifications module migrations applied successfully.");

        await templateSeeder.SeedAsync(cancellationToken);
        logger.LogInformation("Notifications module default templates seeded.");

        await smtpSeeder.SeedAsync(cancellationToken);
        logger.LogInformation("Notifications module default SMTP configuration seed evaluated.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

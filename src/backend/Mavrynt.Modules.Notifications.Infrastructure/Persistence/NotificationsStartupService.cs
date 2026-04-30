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
        var seeder = scope.ServiceProvider.GetRequiredService<DefaultEmailTemplateSeeder>();

        await context.Database.MigrateAsync(cancellationToken);
        logger.LogInformation("Notifications module migrations applied successfully.");

        await seeder.SeedAsync(cancellationToken);
        logger.LogInformation("Notifications module default templates seeded.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

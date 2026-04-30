using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Infrastructure.Persistence;
using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Infrastructure.Email;
using Mavrynt.Modules.Notifications.Infrastructure.Persistence;
using Mavrynt.Modules.Notifications.Infrastructure.Repositories;
using Mavrynt.Modules.Notifications.Infrastructure.Seeding;
using Mavrynt.Modules.Notifications.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mavrynt.Modules.Notifications.Infrastructure.DependencyInjection;

public static class NotificationsInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<NotificationsDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("MavryntDb")
                ?? throw new InvalidOperationException(
                    "Connection string 'MavryntDb' is not configured.");

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "notifications");
            });
        });

        services.AddScoped<ISmtpSettingsRepository, SmtpSettingsRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<NotificationsDbContext>());

        services.AddScoped<DefaultEmailTemplateSeeder>();
        services.AddHostedService<NotificationsStartupService>();

        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddSingleton<ISecretProtector, PassThroughSecretProtector>();

        services.TryAddSingleton<IDateTimeProvider,
            Mavrynt.Modules.Notifications.Infrastructure.Time.UtcDateTimeProvider>();

        return services;
    }
}

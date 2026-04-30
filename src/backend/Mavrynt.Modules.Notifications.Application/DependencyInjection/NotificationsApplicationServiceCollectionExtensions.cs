using Mavrynt.BuildingBlocks.Application.DependencyInjection;
using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.Modules.Notifications.Application.DependencyInjection;

public static class NotificationsApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationsApplication(this IServiceCollection services)
    {
        services.AddCommandAndQueryHandlers<INotificationsApplicationMarker>();
        services.AddScoped<IEmailTemplateRenderer, EmailTemplateRenderer>();
        services.AddScoped<IEmailNotificationService, EmailNotificationService>();

        return services;
    }
}

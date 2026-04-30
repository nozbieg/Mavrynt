using Mavrynt.BuildingBlocks.Application.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.Modules.FeatureManagement.Application.DependencyInjection;

public static class FeatureManagementApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Registers FeatureManagement module application-layer handlers.
    /// Relies on the mediator and pipeline behaviors already registered by AddUsersApplication
    /// (or another module that calls AddMavryntMediator first).
    /// </summary>
    public static IServiceCollection AddFeatureManagementApplication(this IServiceCollection services)
    {
        services.AddCommandAndQueryHandlers<IFeatureManagementApplicationMarker>();

        return services;
    }
}

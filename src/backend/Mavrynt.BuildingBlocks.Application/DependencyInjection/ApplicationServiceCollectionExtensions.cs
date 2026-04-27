using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.BuildingBlocks.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Registers application-layer services.
    /// Concrete mediator, validation, and pipeline behaviors are registered
    /// in module-level DI extensions once those architectural decisions are finalized.
    /// </summary>
    public static IServiceCollection AddApplicationCore(this IServiceCollection services)
    {
        return services;
    }
}

using System.Reflection;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.BuildingBlocks.Application.DependencyInjection;

public static class HandlerRegistrationExtensions
{
    private static readonly Type CommandHandlerOpenType = typeof(ICommandHandler<>);
    private static readonly Type CommandHandlerWithResponseOpenType = typeof(ICommandHandler<,>);
    private static readonly Type QueryHandlerOpenType = typeof(IQueryHandler<,>);

    /// <summary>
    /// Scans the given <paramref name="assembly"/> and registers all concrete
    /// <see cref="ICommandHandler{TCommand}"/>, <see cref="ICommandHandler{TCommand, TResponse}"/>,
    /// and <see cref="IQueryHandler{TQuery, TResponse}"/> implementations as transient services.
    /// </summary>
    public static IServiceCollection AddCommandAndQueryHandlers(
        this IServiceCollection services,
        Assembly assembly)
    {
        var concreteTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false });

        foreach (var type in concreteTypes)
        {
            foreach (var iface in type.GetInterfaces())
            {
                if (!iface.IsGenericType)
                    continue;

                var definition = iface.GetGenericTypeDefinition();

                if (definition == CommandHandlerOpenType ||
                    definition == CommandHandlerWithResponseOpenType ||
                    definition == QueryHandlerOpenType)
                {
                    services.AddTransient(iface, type);
                }
            }
        }

        return services;
    }

    /// <summary>
    /// Scans the assembly containing <typeparamref name="TMarker"/> and registers all concrete
    /// command and query handlers as transient services.
    /// </summary>
    public static IServiceCollection AddCommandAndQueryHandlers<TMarker>(
        this IServiceCollection services)
        => services.AddCommandAndQueryHandlers(typeof(TMarker).Assembly);
}

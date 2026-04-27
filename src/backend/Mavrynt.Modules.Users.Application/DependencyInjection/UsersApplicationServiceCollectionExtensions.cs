using Mavrynt.BuildingBlocks.Application.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.Modules.Users.Application.DependencyInjection;

public static class UsersApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Registers Users module application-layer services.
    /// All <see cref="Mavrynt.BuildingBlocks.Application.Messaging.ICommandHandler{TCommand}"/>,
    /// <see cref="Mavrynt.BuildingBlocks.Application.Messaging.ICommandHandler{TCommand,TResponse}"/>,
    /// and <see cref="Mavrynt.BuildingBlocks.Application.Messaging.IQueryHandler{TQuery,TResponse}"/>
    /// implementations in this assembly are registered automatically.
    /// Infrastructure services (IUserRepository, IUnitOfWork) and
    /// IDateTimeProvider must be registered separately by the host or infrastructure layer.
    /// </summary>
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddCommandAndQueryHandlers<IUserApplicationMarker>();

        return services;
    }
}

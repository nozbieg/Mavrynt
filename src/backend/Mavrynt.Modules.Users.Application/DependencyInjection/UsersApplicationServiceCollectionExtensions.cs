using Mavrynt.BuildingBlocks.Application.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.Modules.Users.Application.DependencyInjection;

public static class UsersApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Registers Users module application-layer services via the Mavrynt internal mediator.
    ///
    /// Registers:
    ///   - <see cref="Mavrynt.BuildingBlocks.Application.Messaging.IMediator"/> (scoped)
    ///   - All command handlers, query handlers, and validators from this assembly
    ///   - Pipeline behaviors: Logging → Validation → Resilience → Audit → Transaction
    ///
    /// Infrastructure services (IUserRepository, IUnitOfWork, IDateTimeProvider)
    /// must be registered separately by the infrastructure layer.
    /// </summary>
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddMavryntMediator(typeof(IUserApplicationMarker).Assembly);

        return services;
    }
}

using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.Commands;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.Modules.Users.Application.DependencyInjection;

public static class UsersApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Registers Users module application-layer services.
    /// Infrastructure services (IUserRepository, IUnitOfWork) and
    /// IDateTimeProvider must be registered separately by the host or infrastructure layer.
    /// </summary>
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        // Command handlers
        services.AddTransient<ICommandHandler<RegisterUserCommand, UserDto>, RegisterUserCommandHandler>();
        services.AddTransient<ICommandHandler<LoginUserCommand, AuthResultDto>, LoginUserCommandHandler>();
        services.AddTransient<ICommandHandler<ChangeUserEmailCommand, UserDto>, ChangeUserEmailCommandHandler>();
        services.AddTransient<ICommandHandler<ChangeUserPasswordCommand>, ChangeUserPasswordCommandHandler>();

        // Query handlers
        services.AddTransient<IQueryHandler<GetUserByIdQuery, UserDto>, GetUserByIdQueryHandler>();
        services.AddTransient<IQueryHandler<GetUserByEmailQuery, UserDto>, GetUserByEmailQueryHandler>();

        return services;
    }
}

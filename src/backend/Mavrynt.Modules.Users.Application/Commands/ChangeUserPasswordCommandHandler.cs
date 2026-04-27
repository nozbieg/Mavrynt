using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed class ChangeUserPasswordCommandHandler : ICommandHandler<ChangeUserPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ChangeUserPasswordCommandHandler(
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> HandleAsync(
        ChangeUserPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = UserId.From(command.UserId);
        if (userIdResult.IsFailure)
            return userIdResult.Error;

        var passwordHashResult = PasswordHash.Create(command.NewPasswordHash);
        if (passwordHashResult.IsFailure)
            return passwordHashResult.Error;

        var user = await _userRepository.GetByIdAsync(userIdResult.Value, cancellationToken);
        if (user is null)
            return UserErrors.UserNotFound;

        user.ChangePasswordHash(passwordHashResult.Value, _dateTimeProvider.UtcNow);

        return Result.Success();
    }
}

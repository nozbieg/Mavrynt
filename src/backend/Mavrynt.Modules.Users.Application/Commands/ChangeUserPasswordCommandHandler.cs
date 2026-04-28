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
    private readonly IPasswordHasher _passwordHasher;

    public ChangeUserPasswordCommandHandler(
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> HandleAsync(
        ChangeUserPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = UserId.From(command.UserId);
        if (userIdResult.IsFailure)
            return userIdResult.Error;

        var user = await _userRepository.GetByIdAsync(userIdResult.Value, cancellationToken);
        if (user is null)
            return UserErrors.UserNotFound;

        // Hash the new raw password before creating the domain value object.
        var hashedPassword = _passwordHasher.HashPassword(command.NewPassword);
        var passwordHashResult = PasswordHash.Create(hashedPassword);
        if (passwordHashResult.IsFailure)
            return passwordHashResult.Error;

        user.ChangePasswordHash(passwordHashResult.Value, _dateTimeProvider.UtcNow);

        return Result.Success();
    }
}

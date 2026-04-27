using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Mapping;
using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed class ChangeUserEmailCommandHandler : ICommandHandler<ChangeUserEmailCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ChangeUserEmailCommandHandler(
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<UserDto>> HandleAsync(
        ChangeUserEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = UserId.From(command.UserId);
        if (userIdResult.IsFailure)
            return userIdResult.Error;

        var emailResult = Email.Create(command.NewEmail);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var user = await _userRepository.GetByIdAsync(userIdResult.Value, cancellationToken);
        if (user is null)
            return UserErrors.UserNotFound;

        var emailTaken = await _userRepository.ExistsByEmailAsync(emailResult.Value, cancellationToken);
        if (emailTaken)
            return UserErrors.EmailAlreadyTaken;

        user.ChangeEmail(emailResult.Value, _dateTimeProvider.UtcNow);

        return user.ToDto();
    }
}

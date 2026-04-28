using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Mapping;
using Mavrynt.Modules.Users.Domain.Entities;
using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditService _auditService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider,
        IPasswordHasher passwordHasher,
        IAuditService auditService)
    {
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
        _passwordHasher = passwordHasher;
        _auditService = auditService;
    }

    public async Task<Result<UserDto>> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var emailExists = await _userRepository.ExistsByEmailAsync(emailResult.Value, cancellationToken);
        if (emailExists)
            return UserErrors.EmailAlreadyTaken;

        UserDisplayName? displayName = null;
        if (command.DisplayName is not null)
        {
            var displayNameResult = UserDisplayName.Create(command.DisplayName);
            if (displayNameResult.IsFailure)
                return displayNameResult.Error;

            displayName = displayNameResult.Value;
        }

        // Hash the raw password — the hash is what gets stored, never the plain text.
        var hashedPassword = _passwordHasher.HashPassword(command.Password);
        var passwordHashResult = PasswordHash.Create(hashedPassword);
        if (passwordHashResult.IsFailure)
            return passwordHashResult.Error;

        var userIdResult = UserId.New();
        if (userIdResult.IsFailure)
            return userIdResult.Error;

        var now = _dateTimeProvider.UtcNow;

        var user = User.Register(
            userIdResult.Value,
            emailResult.Value,
            passwordHashResult.Value,
            displayName,
            now);

        await _userRepository.AddAsync(user, cancellationToken);

        await _auditService.RecordAsync(new AuditEntry(
            EventType: "user_registered",
            OccurredAt: now,
            UserId: user.Id.Value,
            Email: user.Email.Value),
            cancellationToken);

        return user.ToDto();
    }
}

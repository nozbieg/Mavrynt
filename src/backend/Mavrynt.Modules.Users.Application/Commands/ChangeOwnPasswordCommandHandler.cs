using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed class ChangeOwnPasswordCommandHandler : ICommandHandler<ChangeOwnPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditService _auditService;

    public ChangeOwnPasswordCommandHandler(
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

    public async Task<Result> HandleAsync(
        ChangeOwnPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = UserId.From(command.UserId);
        if (userIdResult.IsFailure)
            return userIdResult.Error;

        var user = await _userRepository.GetByIdAsync(userIdResult.Value, cancellationToken);
        if (user is null)
            return UserErrors.UserNotFound;

        var currentPasswordValid = _passwordHasher.VerifyPassword(command.CurrentPassword, user.PasswordHash.Value);
        if (!currentPasswordValid)
            return UserErrors.InvalidCurrentPassword;

        var hashedPassword = _passwordHasher.HashPassword(command.NewPassword);
        var passwordHashResult = PasswordHash.Create(hashedPassword);
        if (passwordHashResult.IsFailure)
            return passwordHashResult.Error;

        var now = _dateTimeProvider.UtcNow;
        user.ChangePasswordHash(passwordHashResult.Value, now);

        await _auditService.RecordAsync(new AuditEntry(
            EventType: "password_changed",
            OccurredAt: now,
            UserId: user.Id.Value,
            Email: user.Email.Value),
            cancellationToken);

        return Result.Success();
    }
}

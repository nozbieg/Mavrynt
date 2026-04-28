using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.Abstractions;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Mapping;
using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuditService _auditService;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IDateTimeProvider dateTimeProvider,
        IAuditService auditService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _dateTimeProvider = dateTimeProvider;
        _auditService = auditService;
    }

    public async Task<Result<AuthResultDto>> HandleAsync(
        LoginUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var now = _dateTimeProvider.UtcNow;

        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            // Return generic error — do not reveal that email format was invalid.
            await RecordFailedLoginAsync(command.Email, now, cancellationToken);
            return UserErrors.InvalidCredentials;
        }

        var user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null)
        {
            await RecordFailedLoginAsync(command.Email, now, cancellationToken);
            return UserErrors.InvalidCredentials;
        }

        var passwordValid = _passwordHasher.VerifyPassword(command.Password, user.PasswordHash.Value);
        if (!passwordValid)
        {
            await RecordFailedLoginAsync(command.Email, now, cancellationToken);
            return UserErrors.InvalidCredentials;
        }

        var tokenResult = _jwtTokenService.GenerateToken(
            user.Id.Value,
            user.Email.Value,
            user.DisplayName?.Value,
            user.Role.ToString());

        await _auditService.RecordAsync(new AuditEntry(
            EventType: "login_success",
            OccurredAt: now,
            UserId: user.Id.Value,
            Email: user.Email.Value),
            cancellationToken);

        return new AuthResultDto(
            user.ToDto(),
            tokenResult.Token,
            "Bearer",
            tokenResult.ExpiresAt);
    }

    private Task RecordFailedLoginAsync(
        string attemptedEmail,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken)
        => _auditService.RecordAsync(new AuditEntry(
            EventType: "login_failed",
            OccurredAt: occurredAt,
            Email: attemptedEmail),
            cancellationToken);
}

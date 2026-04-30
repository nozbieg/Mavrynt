using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Mapping;
using Mavrynt.Modules.Users.Domain.Enums;
using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed class AssignUserRoleCommandHandler : ICommandHandler<AssignUserRoleCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuditService _auditService;

    public AssignUserRoleCommandHandler(
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider,
        IAuditService auditService)
    {
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
        _auditService = auditService;
    }

    public async Task<Result<UserDto>> HandleAsync(
        AssignUserRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = UserId.From(command.UserId);
        if (userIdResult.IsFailure)
            return userIdResult.Error;

        if (!Enum.TryParse<UserRole>(command.Role, ignoreCase: true, out var role))
            return UserErrors.InvalidRole;

        var user = await _userRepository.GetByIdAsync(userIdResult.Value, cancellationToken);
        if (user is null)
            return UserErrors.UserNotFound;

        user.AssignRole(role, _dateTimeProvider.UtcNow);

        await _auditService.RecordAsync(new AuditEntry(
            EventType: "user_role_assigned",
            OccurredAt: _dateTimeProvider.UtcNow,
            UserId: user.Id.Value,
            Metadata: $"{{\"newRole\":\"{role}\"}}"),
            cancellationToken);

        return user.ToDto();
    }
}

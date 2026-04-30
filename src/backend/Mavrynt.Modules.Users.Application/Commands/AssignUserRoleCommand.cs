using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.DTOs;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed record AssignUserRoleCommand(
    Guid UserId,
    string Role
) : ICommand<UserDto>;

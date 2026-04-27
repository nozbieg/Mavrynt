using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.DTOs;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed record RegisterUserCommand(
    string Email,
    string PasswordHash,
    string? DisplayName
) : ICommand<UserDto>;

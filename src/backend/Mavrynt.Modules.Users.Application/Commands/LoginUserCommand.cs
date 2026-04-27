using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.DTOs;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed record LoginUserCommand(
    string Email,
    string PasswordHash
) : ICommand<AuthResultDto>;

using Mavrynt.BuildingBlocks.Application.Messaging;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed record ChangeUserPasswordCommand(
    Guid UserId,
    string NewPasswordHash
) : ICommand;

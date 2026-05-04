using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.DTOs;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed record ChangeUserEmailCommand(
    Guid UserId,
    string NewEmail
) : ICommand<UserDto>, ITransactionalRequest;

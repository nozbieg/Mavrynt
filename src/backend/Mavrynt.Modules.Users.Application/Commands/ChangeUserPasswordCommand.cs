using Mavrynt.BuildingBlocks.Application.Messaging;

namespace Mavrynt.Modules.Users.Application.Commands;

/// <summary>
/// Changes a user's password. The raw <see cref="NewPassword"/> is hashed by the handler.
/// It must never be logged or persisted as-is.
/// </summary>
public sealed record ChangeUserPasswordCommand(
    Guid UserId,
    string NewPassword
) : ICommand;

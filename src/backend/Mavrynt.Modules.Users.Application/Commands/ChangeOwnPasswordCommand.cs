using Mavrynt.BuildingBlocks.Application.Messaging;

namespace Mavrynt.Modules.Users.Application.Commands;

/// <summary>
/// Self-service password change. Verifies <see cref="CurrentPassword"/> before applying the new hash.
/// Raw passwords must never be logged or persisted.
/// </summary>
public sealed record ChangeOwnPasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : ICommand;

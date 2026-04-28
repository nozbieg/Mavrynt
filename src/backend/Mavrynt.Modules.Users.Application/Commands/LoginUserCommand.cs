using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.DTOs;

namespace Mavrynt.Modules.Users.Application.Commands;

/// <summary>
/// Authenticates a user by email and raw password.
/// The raw <see cref="Password"/> must never be logged or persisted.
/// </summary>
public sealed record LoginUserCommand(
    string Email,
    string Password
) : ICommand<AuthResultDto>;

using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.DTOs;

namespace Mavrynt.Modules.Users.Application.Commands;

/// <summary>
/// Registers a new user. The raw <see cref="Password"/> is hashed by the handler
/// before being stored — it must never be logged or persisted as-is.
/// </summary>
public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string? DisplayName
) : ICommand<UserDto>, ITransactionalRequest;

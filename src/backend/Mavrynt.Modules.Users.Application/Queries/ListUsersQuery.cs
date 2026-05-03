using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.DTOs;

namespace Mavrynt.Modules.Users.Application.Queries;

public sealed record ListUsersQuery() : IQuery<IReadOnlyList<UserDto>>;

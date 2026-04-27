using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.DTOs;

namespace Mavrynt.Modules.Users.Application.Queries;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserDto>;

using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Queries;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed record AssignUserRoleCommand(
    Guid UserId,
    string Role
) : ICommand<UserDto>, ITransactionalRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheKeysToInvalidate => [UsersCacheKeys.UserById(UserId), UsersCacheKeys.UsersList];
    public IReadOnlyCollection<string> CacheTagsToInvalidate => ["users", $"users:user:{UserId:N}"];
}

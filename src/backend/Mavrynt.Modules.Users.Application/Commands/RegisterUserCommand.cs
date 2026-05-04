using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Queries;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string? DisplayName
) : ICommand<UserDto>, ITransactionalRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheKeysToInvalidate => [UsersCacheKeys.UserByEmail(Email), UsersCacheKeys.UsersList];
    public IReadOnlyCollection<string> CacheTagsToInvalidate => ["users", $"users:email:{Email.Trim().ToLowerInvariant()}"];
}

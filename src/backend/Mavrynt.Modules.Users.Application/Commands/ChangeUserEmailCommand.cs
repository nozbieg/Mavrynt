using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Queries;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed record ChangeUserEmailCommand(
    Guid UserId,
    string NewEmail
) : ICommand<UserDto>, ITransactionalRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheKeysToInvalidate => [UsersCacheKeys.UserById(UserId), UsersCacheKeys.UserByEmail(NewEmail), UsersCacheKeys.UsersList];
    public IReadOnlyCollection<string> CacheTagsToInvalidate => ["users", $"users:user:{UserId:N}", $"users:email:{NewEmail.Trim().ToLowerInvariant()}"];
}

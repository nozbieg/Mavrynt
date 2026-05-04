using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.Queries;

namespace Mavrynt.Modules.Users.Application.Commands;

/// <summary>
/// Changes a user's password. The raw <see cref="NewPassword"/> is hashed by the handler.
/// It must never be logged or persisted as-is.
/// </summary>
public sealed record ChangeUserPasswordCommand(
    Guid UserId,
    string NewPassword
) : ICommand, ITransactionalRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheKeysToInvalidate => [UsersCacheKeys.UserById(UserId), UsersCacheKeys.UsersList];
    public IReadOnlyCollection<string> CacheTagsToInvalidate => ["users", $"users:user:{UserId:N}"];
}

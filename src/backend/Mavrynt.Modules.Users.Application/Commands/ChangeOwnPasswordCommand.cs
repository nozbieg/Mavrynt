using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.Queries;

namespace Mavrynt.Modules.Users.Application.Commands;

/// <summary>
/// Self-service password change. Verifies <see cref="CurrentPassword"/> before applying the new hash.
/// Raw passwords must never be logged or persisted.
/// </summary>
public sealed record ChangeOwnPasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : ICommand, ITransactionalRequest, IInvalidatesCache
{
    public IReadOnlyCollection<string> CacheKeysToInvalidate => [UsersCacheKeys.UserById(UserId), UsersCacheKeys.UsersList];
    public IReadOnlyCollection<string> CacheTagsToInvalidate => ["users", $"users:user:{UserId:N}"];
}

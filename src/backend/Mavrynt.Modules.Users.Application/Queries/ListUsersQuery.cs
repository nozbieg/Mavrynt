using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.Modules.Users.Application.Dtos;

namespace Mavrynt.Modules.Users.Application.Queries;

public sealed record ListUsersQuery() : ICachedQuery<IReadOnlyList<UserDto>>
{
    public string CacheKey => UsersCacheKeys.UsersList;
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    public IReadOnlyCollection<string> CacheTags => ["users"];
}

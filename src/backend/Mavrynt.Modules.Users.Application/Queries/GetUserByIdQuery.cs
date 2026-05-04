using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.Modules.Users.Application.Dtos;

namespace Mavrynt.Modules.Users.Application.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : ICachedQuery<UserDto>
{
    public string CacheKey => UsersCacheKeys.UserById(UserId);
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(5);
    public IReadOnlyCollection<string> CacheTags => ["users", $"users:user:{UserId:N}"];
}

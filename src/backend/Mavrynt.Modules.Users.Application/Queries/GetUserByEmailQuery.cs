using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Users.Application.Dtos;

namespace Mavrynt.Modules.Users.Application.Queries;

public sealed record GetUserByEmailQuery(string Email) : ICachedQuery<UserDto>
{
    public string CacheKey => UsersCacheKeys.UserByEmail(Email);
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(5);
    public IReadOnlyCollection<string> CacheTags => ["users", $"users:email:{Email.Trim().ToLowerInvariant()}"];
}

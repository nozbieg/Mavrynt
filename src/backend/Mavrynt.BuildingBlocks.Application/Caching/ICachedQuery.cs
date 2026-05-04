using Mavrynt.BuildingBlocks.Application.Messaging;

namespace Mavrynt.BuildingBlocks.Application.Caching;

public interface ICachedQuery<TResponse> : IQuery<TResponse>
{
    string CacheKey { get; }
    TimeSpan? CacheDuration { get; }
    IReadOnlyCollection<string> CacheTags { get; }
}

namespace Mavrynt.BuildingBlocks.Application.Caching;

public interface ICacheService
{
    Task<CacheValue<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T? value, CacheEntryOptions? options = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default);
    Task RemoveByTagsAsync(IReadOnlyCollection<string> tags, CancellationToken cancellationToken = default);
}

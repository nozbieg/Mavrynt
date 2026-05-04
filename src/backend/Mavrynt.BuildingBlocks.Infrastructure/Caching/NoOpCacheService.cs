using Mavrynt.BuildingBlocks.Application.Caching;

namespace Mavrynt.BuildingBlocks.Infrastructure.Caching;

public sealed class NoOpCacheService : ICacheService
{
    public Task<CacheValue<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default) => Task.FromResult(new CacheValue<T>(false, default));
    public Task SetAsync<T>(string key, T? value, CacheEntryOptions? options = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task RemoveByTagsAsync(IReadOnlyCollection<string> tags, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

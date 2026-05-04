using System.Text.Json;
using Mavrynt.BuildingBlocks.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Mavrynt.BuildingBlocks.Infrastructure.Caching;

public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache; private readonly IConnectionMultiplexer _redis; private readonly MavryntCacheOptions _options;
    public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redis, IOptions<MavryntCacheOptions> options){_cache=cache;_redis=redis;_options=options.Value;}
    static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    string CK(string key)=>$"{_options.KeyPrefix}:cache:{key}"; string TK(string tag)=>$"{_options.KeyPrefix}:cache-tag:{tag}";
    public async Task<CacheValue<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default){var s=await _cache.GetStringAsync(CK(key),cancellationToken); return s is null?new(false,default):new(true,JsonSerializer.Deserialize<T>(s,JsonOptions));}
    public async Task SetAsync<T>(string key, T? value, CacheEntryOptions? options = null, CancellationToken cancellationToken = default){var o=options?.AbsoluteExpiration ?? TimeSpan.FromMinutes(_options.DefaultAbsoluteExpirationMinutes); await _cache.SetStringAsync(CK(key),JsonSerializer.Serialize(value,JsonOptions),new DistributedCacheEntryOptions{AbsoluteExpirationRelativeToNow=o},cancellationToken); var db=_redis.GetDatabase(); foreach(var t in options?.Tags??[]){await db.SetAddAsync(TK(t), CK(key));}}
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)=>_cache.RemoveAsync(CK(key),cancellationToken);
    public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default){var db=_redis.GetDatabase();var tk=TK(tag);var keys=await db.SetMembersAsync(tk); if(keys.Length>0){foreach(var k in keys) await _cache.RemoveAsync(k!,cancellationToken);} await db.KeyDeleteAsync(tk);}    
    public async Task RemoveByTagsAsync(IReadOnlyCollection<string> tags, CancellationToken cancellationToken = default){foreach(var t in tags) await RemoveByTagAsync(t,cancellationToken);}    
}

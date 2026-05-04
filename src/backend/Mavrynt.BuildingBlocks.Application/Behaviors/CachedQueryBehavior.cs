using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Domain.Results;
using Microsoft.Extensions.Logging;

namespace Mavrynt.BuildingBlocks.Application.Behaviors;

public sealed class CachedQueryBehavior<TRequest, TResponse> : IMavryntBehavior<TRequest, Result<TResponse>>
    where TRequest : notnull
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedQueryBehavior<TRequest, TResponse>> _logger;

    public CachedQueryBehavior(ICacheService cacheService, ILogger<CachedQueryBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<TResponse>> HandleAsync(TRequest request, Func<CancellationToken, Task<Result<TResponse>>> next, CancellationToken cancellationToken)
    {
        if (request is not ICachedQuery<TResponse> cachedQuery)
            return await next(cancellationToken);

        var cached = await _cacheService.GetAsync<TResponse>(cachedQuery.CacheKey, cancellationToken);
        if (cached.HasValue)
        {
            _logger.LogDebug("Cache hit for {RequestType}", typeof(TRequest).Name);
            return Result.Success(cached.Value!);
        }

        var response = await next(cancellationToken);
        if (response.IsSuccess)
        {
            await _cacheService.SetAsync(cachedQuery.CacheKey, response.Value,
                new CacheEntryOptions(cachedQuery.CacheDuration, cachedQuery.CacheTags), cancellationToken);
        }

        return response;
    }
}

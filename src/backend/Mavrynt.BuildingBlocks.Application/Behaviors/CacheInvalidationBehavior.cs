using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.BuildingBlocks.Application.Behaviors;

public sealed class CacheInvalidationBehavior<TRequest, TResponse> : IMavryntBehavior<TRequest, TResponse>
    where TResponse : notnull
{
    private readonly ICacheService _cacheService;

    public CacheInvalidationBehavior(ICacheService cacheService) => _cacheService = cacheService;

    public async Task<TResponse> HandleAsync(TRequest request, Func<CancellationToken, Task<TResponse>> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        if (request is not IInvalidatesCache invalidates || response is not Result result || !result.IsSuccess)
            return response;

        foreach (var key in invalidates.CacheKeysToInvalidate)
            await _cacheService.RemoveAsync(key, cancellationToken);

        await _cacheService.RemoveByTagsAsync(invalidates.CacheTagsToInvalidate, cancellationToken);
        return response;
    }
}

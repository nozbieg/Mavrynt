using Mavrynt.BuildingBlocks.Application.Caching;
using Mavrynt.BuildingBlocks.Infrastructure.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Mavrynt.BuildingBlocks.Infrastructure.DependencyInjection;

public static class CachingServiceCollectionExtensions
{
    public static IServiceCollection AddMavryntCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MavryntCacheOptions>(configuration.GetSection("Cache"));
        var options = configuration.GetSection("Cache").Get<MavryntCacheOptions>() ?? new MavryntCacheOptions();
        if (!options.Enabled){services.AddSingleton<ICacheService, NoOpCacheService>(); return services;}
        services.AddStackExchangeRedisCache(o => o.Configuration = configuration.GetSection("Redis")["ConnectionString"]);
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(configuration.GetSection("Redis")["ConnectionString"]!));
        services.AddSingleton<ICacheService, RedisCacheService>();
        return services;
    }
}

namespace Mavrynt.BuildingBlocks.Application.Caching;

public readonly record struct CacheValue<T>(bool HasValue, T? Value);

namespace Mavrynt.BuildingBlocks.Application.Caching;

public sealed record CacheEntryOptions(TimeSpan? AbsoluteExpiration = null, IReadOnlyCollection<string>? Tags = null);

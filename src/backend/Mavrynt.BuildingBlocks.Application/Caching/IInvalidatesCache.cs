namespace Mavrynt.BuildingBlocks.Application.Caching;

public interface IInvalidatesCache
{
    IReadOnlyCollection<string> CacheKeysToInvalidate { get; }
    IReadOnlyCollection<string> CacheTagsToInvalidate { get; }
}

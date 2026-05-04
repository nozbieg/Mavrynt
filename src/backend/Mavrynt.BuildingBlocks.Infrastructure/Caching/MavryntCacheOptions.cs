namespace Mavrynt.BuildingBlocks.Infrastructure.Caching;

public sealed class MavryntCacheOptions
{
    public bool Enabled { get; set; } = true;
    public int DefaultAbsoluteExpirationMinutes { get; set; } = 5;
    public string KeyPrefix { get; set; } = "mavrynt";
}

namespace Mavrynt.BuildingBlocks.Infrastructure.Configuration;

public sealed class PostgreSqlOptions
{
    public const string SectionName = "PostgreSql";

    public string ConnectionString { get; init; } = string.Empty;
    public bool EnableSensitiveDataLogging { get; init; } = false;
    public int CommandTimeoutSeconds { get; init; } = 30;
}

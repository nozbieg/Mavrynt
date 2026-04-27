using Mavrynt.BuildingBlocks.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;

namespace Mavrynt.BuildingBlocks.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Reads and binds a <see cref="PostgreSqlOptions"/> instance from the given configuration section.
    /// Falls back to <see cref="PostgreSqlOptions.SectionName"/> when no section name is provided.
    /// </summary>
    public static PostgreSqlOptions GetPostgreSqlOptions(
        this IConfiguration configuration,
        string sectionName = PostgreSqlOptions.SectionName)
    {
        var section = configuration.GetSection(sectionName);
        return new PostgreSqlOptions
        {
            ConnectionString = section["ConnectionString"] ?? string.Empty,
            EnableSensitiveDataLogging = bool.TryParse(section["EnableSensitiveDataLogging"], out var sensitive) && sensitive,
            CommandTimeoutSeconds = int.TryParse(section["CommandTimeoutSeconds"], out var timeout) ? timeout : 30
        };
    }
}

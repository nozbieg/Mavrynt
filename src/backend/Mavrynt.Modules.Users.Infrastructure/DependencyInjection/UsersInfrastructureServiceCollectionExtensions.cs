using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Infrastructure.Persistence;
using Mavrynt.Modules.Users.Application.Abstractions;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Infrastructure.Persistence;
using Mavrynt.Modules.Users.Infrastructure.Repositories;
using Mavrynt.Modules.Users.Infrastructure.Security;
using Mavrynt.Modules.Users.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mavrynt.Modules.Users.Infrastructure.DependencyInjection;

public static class UsersInfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Users module infrastructure services:
    /// EF Core DbContext, repository, password hasher, JWT token service, and date/time provider.
    /// </summary>
    public static IServiceCollection AddUsersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Persistence ───────────────────────────────────────────────────────
        services.AddDbContext<UsersDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("MavryntDb")
                ?? throw new InvalidOperationException(
                    "Connection string 'MavryntDb' is not configured. " +
                    "Add it under ConnectionStrings:MavryntDb in appsettings.");

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "users");
            });

            if (configuration.GetValue<bool>("PostgreSql:EnableSensitiveDataLogging"))
                options.EnableSensitiveDataLogging();
        });

        services.AddScoped<IUserRepository, UserRepository>();

        // Expose the scoped DbContext as IUnitOfWork for callers that need
        // explicit commit control over multi-aggregate operations.
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UsersDbContext>());

        // ── JWT ───────────────────────────────────────────────────────────────
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // ── Security / time ───────────────────────────────────────────────────
        services.AddSingleton<IPasswordHasher, AspNetPasswordHasher>();
        services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();

        return services;
    }
}

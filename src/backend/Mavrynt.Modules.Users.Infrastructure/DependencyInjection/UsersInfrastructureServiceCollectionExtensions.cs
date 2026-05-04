using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Infrastructure.Persistence;
using Mavrynt.Modules.Users.Application.Abstractions;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Infrastructure.Audit;
using Mavrynt.Modules.Users.Infrastructure.Persistence;
using Mavrynt.Modules.Users.Infrastructure.Repositories;
using Mavrynt.Modules.Users.Infrastructure.Security;
using Mavrynt.Modules.Users.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        services.AddScoped<IAuditService, EfAuditService>();

        // Expose the scoped DbContext as IUnitOfWork for the mediator pipeline
        // (TransactionBehavior) and any caller that needs explicit commit control.
        // Registered against the Application-layer interface — that is the contract
        // TransactionBehavior resolves; the Infrastructure marker interface adds nothing.
        services.AddScoped<Mavrynt.BuildingBlocks.Application.Persistence.IUnitOfWork>(
            sp => sp.GetRequiredService<UsersDbContext>());

        // Applies any pending EF Core migrations on startup before the host
        // begins accepting requests. Safe to run in both Api and AdminApp —
        // MigrateAsync is idempotent and uses an advisory lock in PostgreSQL.
        services.AddHostedService<DatabaseMigrationService>();

        // ── JWT ───────────────────────────────────────────────────────────────
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // ── Security / time ───────────────────────────────────────────────────
        services.AddSingleton<IPasswordHasher, AspNetPasswordHasher>();
        services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();

        return services;
    }
}

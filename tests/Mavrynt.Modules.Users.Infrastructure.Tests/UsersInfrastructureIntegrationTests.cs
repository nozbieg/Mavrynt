using Mavrynt.BuildingBlocks.Infrastructure.Persistence;
using Mavrynt.Modules.Users.Domain.Entities;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Domain.ValueObjects;
using Mavrynt.Modules.Users.Infrastructure.DependencyInjection;
using Mavrynt.Modules.Users.Infrastructure.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Mavrynt.Modules.Users.Infrastructure.Tests;

[Collection(PostgreSqlCollection.Name)]
public sealed class UsersInfrastructureIntegrationTests(PostgreSqlContainerFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => fixture.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DbContext_Should_Connect_And_Create_Schema()
    {
        await using var context = fixture.CreateDbContext();
        Assert.True(await context.Database.CanConnectAsync());
        Assert.NotNull(context.Users);
    }

    [Fact]
    public async Task Repository_Should_Add_And_Get_User_By_Id_And_Email()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var user = CreateUser("john@example.com");
        await repository.AddAsync(user);

        var byId = await repository.GetByIdAsync(user.Id);
        var byEmail = await repository.GetByEmailAsync(user.Email);

        Assert.NotNull(byId);
        Assert.NotNull(byEmail);
        Assert.Equal(user.Id, byEmail!.Id);
    }

    [Fact]
    public async Task Repository_Should_Return_Null_For_Missing_User()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        Assert.Null(await repository.GetByIdAsync(UserId.New().Value));
        Assert.Null(await repository.GetByEmailAsync(Email.Create("missing@example.com").Value));
    }

    [Fact]
    public async Task User_Changes_Should_Be_Persisted_After_UnitOfWork_SaveChanges()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var user = CreateUser("john@example.com");
        await repository.AddAsync(user);
        user.ChangeEmail(Email.Create("new@example.com").Value, DateTimeOffset.UtcNow);
        await unitOfWork.SaveChangesAsync();

        Assert.NotNull(await repository.GetByEmailAsync(Email.Create("new@example.com").Value));
    }

    [Fact]
    public async Task Unique_Email_Constraint_Should_Be_Enforced()
    {
        await using var context = fixture.CreateDbContext();
        context.Users.Add(CreateUser("duplicate@example.com"));
        context.Users.Add(CreateUser("duplicate@example.com"));

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    private static User CreateUser(string email) =>
        User.Register(UserId.New().Value, Email.Create(email).Value, PasswordHash.Create("hash").Value, UserDisplayName.Create("John").Value, DateTimeOffset.UtcNow);

    private AsyncServiceScope CreateScope()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MavryntDb"] = fixture.ConnectionString,
                ["Jwt:Issuer"] = "tests",
                ["Jwt:Audience"] = "tests",
                ["Jwt:SigningKey"] = "0123456789abcdef0123456789abcdef",
                ["Jwt:AccessTokenLifetimeMinutes"] = "60"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddUsersInfrastructure(config);
        return services.BuildServiceProvider().CreateAsyncScope();
    }
}

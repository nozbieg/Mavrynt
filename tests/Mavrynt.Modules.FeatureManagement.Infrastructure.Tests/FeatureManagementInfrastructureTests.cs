using Mavrynt.Modules.FeatureManagement.Domain.Entities;
using Mavrynt.Modules.FeatureManagement.Domain.Repositories;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;
using Mavrynt.Modules.FeatureManagement.Infrastructure.DependencyInjection;
using Mavrynt.Modules.FeatureManagement.Infrastructure.Persistence;
using Mavrynt.Modules.FeatureManagement.Infrastructure.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Mavrynt.Modules.FeatureManagement.Infrastructure.Tests;

[Collection(PostgreSqlCollection.Name)]
public sealed class FeatureManagementInfrastructureTests(PostgreSqlContainerFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => fixture.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DbContext_Should_Connect_And_Create_Schema()
    {
        await using var context = fixture.CreateDbContext();
        Assert.True(await context.Database.CanConnectAsync());
        Assert.NotNull(context.FeatureFlags);
    }

    [Fact]
    public async Task Repository_Should_Add_And_Get_Flag_By_Key()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IFeatureFlagRepository>();
        var context = scope.ServiceProvider.GetRequiredService<FeatureManagementDbContext>();
        var flag = CreateFlag("infra.test-flag");
        await repository.AddAsync(flag);
        await context.SaveChangesAsync();

        var found = await repository.GetByKeyAsync(FeatureFlagKey.Create("infra.test-flag").Value);

        Assert.NotNull(found);
        Assert.Equal("infra.test-flag", found!.Key.Value);
    }

    [Fact]
    public async Task Repository_Should_Return_Null_For_Missing_Flag()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IFeatureFlagRepository>();

        var result = await repository.GetByKeyAsync(FeatureFlagKey.Create("not.found").Value);

        Assert.Null(result);
    }

    [Fact]
    public async Task Unique_Key_Constraint_Should_Be_Enforced()
    {
        await using var context = fixture.CreateDbContext();
        context.FeatureFlags.Add(CreateFlag("duplicate.key"));
        context.FeatureFlags.Add(CreateFlag("duplicate.key"));

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task Flag_Update_Should_Be_Persisted()
    {
        await using var scope1 = CreateScope();
        var repository1 = scope1.ServiceProvider.GetRequiredService<IFeatureFlagRepository>();
        var context1 = scope1.ServiceProvider.GetRequiredService<FeatureManagementDbContext>();
        await repository1.AddAsync(CreateFlag("update.test"));
        await context1.SaveChangesAsync();

        await using var scope2 = CreateScope();
        var repository2 = scope2.ServiceProvider.GetRequiredService<IFeatureFlagRepository>();
        var context2 = scope2.ServiceProvider.GetRequiredService<FeatureManagementDbContext>();
        var loaded = await repository2.GetByKeyAsync(FeatureFlagKey.Create("update.test").Value);
        loaded!.UpdateDetails("Updated Name", "Updated Desc", DateTimeOffset.UtcNow);
        await context2.SaveChangesAsync();

        await using var scope3 = CreateScope();
        var repository3 = scope3.ServiceProvider.GetRequiredService<IFeatureFlagRepository>();
        var verified = await repository3.GetByKeyAsync(FeatureFlagKey.Create("update.test").Value);
        Assert.Equal("Updated Name", verified!.Name);
    }

    [Fact]
    public async Task Repository_List_Should_Return_All_Flags()
    {
        await using var scope = CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IFeatureFlagRepository>();
        var context = scope.ServiceProvider.GetRequiredService<FeatureManagementDbContext>();
        await repository.AddAsync(CreateFlag("list.flag-a"));
        await repository.AddAsync(CreateFlag("list.flag-b"));
        await context.SaveChangesAsync();

        var all = await repository.ListAsync();

        Assert.Equal(2, all.Count);
    }

    private static FeatureFlag CreateFlag(string key) =>
        FeatureFlag.Create(
            FeatureFlagId.New().Value,
            FeatureFlagKey.Create(key).Value,
            "Test Flag",
            null,
            true,
            DateTimeOffset.UtcNow).Value;

    private AsyncServiceScope CreateScope()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MavryntDb"] = fixture.ConnectionString
            })
            .Build();

        var services = new ServiceCollection();
        services.AddFeatureManagementInfrastructure(config);
        return services.BuildServiceProvider().CreateAsyncScope();
    }
}

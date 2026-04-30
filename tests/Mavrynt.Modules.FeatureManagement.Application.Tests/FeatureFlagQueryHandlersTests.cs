using Mavrynt.Modules.FeatureManagement.Application.Queries;
using Mavrynt.Modules.FeatureManagement.Application.Tests.Fakes;
using Mavrynt.Modules.FeatureManagement.Domain.Entities;
using Mavrynt.Modules.FeatureManagement.Domain.Errors;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;
using Xunit;

namespace Mavrynt.Modules.FeatureManagement.Application.Tests;

public sealed class FeatureFlagQueryHandlersTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    [Fact]
    public async Task List_Should_Return_All_Flags()
    {
        var repo = new InMemoryFeatureFlagRepository();
        repo.Seed(CreateFlag("flag.one"));
        repo.Seed(CreateFlag("flag.two"));
        var handler = new ListFeatureFlagsQueryHandler(repo);

        var result = await handler.HandleAsync(new ListFeatureFlagsQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task List_Should_Return_Empty_When_No_Flags()
    {
        var handler = new ListFeatureFlagsQueryHandler(new InMemoryFeatureFlagRepository());

        var result = await handler.HandleAsync(new ListFeatureFlagsQuery());

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetByKey_Should_Return_Flag_When_Exists()
    {
        var repo = new InMemoryFeatureFlagRepository();
        repo.Seed(CreateFlag("find.me"));
        var handler = new GetFeatureFlagByKeyQueryHandler(repo);

        var result = await handler.HandleAsync(new GetFeatureFlagByKeyQuery("find.me"));

        Assert.True(result.IsSuccess);
        Assert.Equal("find.me", result.Value.Key);
    }

    [Fact]
    public async Task GetByKey_Should_Return_NotFound_When_Missing()
    {
        var handler = new GetFeatureFlagByKeyQueryHandler(new InMemoryFeatureFlagRepository());

        var result = await handler.HandleAsync(new GetFeatureFlagByKeyQuery("missing.flag"));

        Assert.True(result.IsFailure);
        Assert.Same(FeatureManagementErrors.FeatureFlagNotFound, result.Error);
    }

    [Fact]
    public async Task GetByKey_Should_Return_Error_For_Invalid_Key()
    {
        var handler = new GetFeatureFlagByKeyQueryHandler(new InMemoryFeatureFlagRepository());

        var result = await handler.HandleAsync(new GetFeatureFlagByKeyQuery("INVALID KEY!"));

        Assert.True(result.IsFailure);
    }

    private static FeatureFlag CreateFlag(string key) =>
        FeatureFlag.Create(
            FeatureFlagId.New().Value,
            FeatureFlagKey.Create(key).Value,
            "Test Flag",
            null,
            true,
            Now).Value;
}

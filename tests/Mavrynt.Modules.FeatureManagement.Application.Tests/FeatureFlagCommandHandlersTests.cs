using Mavrynt.Modules.FeatureManagement.Application.Commands;
using Mavrynt.Modules.FeatureManagement.Application.Tests.Fakes;
using Mavrynt.Modules.FeatureManagement.Domain.Entities;
using Mavrynt.Modules.FeatureManagement.Domain.Errors;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;
using Xunit;

namespace Mavrynt.Modules.FeatureManagement.Application.Tests;

public sealed class FeatureFlagCommandHandlersTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    // ── CreateFeatureFlagCommand ───────────────────────────────────────────────

    [Fact]
    public async Task Create_Should_Persist_Flag_And_Write_Audit()
    {
        var repo = new InMemoryFeatureFlagRepository();
        var audit = new FakeAuditLogWriter();
        var handler = new CreateFeatureFlagCommandHandler(repo, new FixedDateTimeProvider(Now), audit);

        var result = await handler.HandleAsync(
            new CreateFeatureFlagCommand("users.test-flag", "Test", null, true));

        Assert.True(result.IsSuccess);
        Assert.Single(repo.Flags);
        Assert.Equal("users.test-flag", repo.Flags[0].Key.Value);
        Assert.Single(audit.Entries, e => e.Action == "FeatureFlagCreated");
    }

    [Fact]
    public async Task Create_Should_Fail_For_Duplicate_Key()
    {
        var repo = new InMemoryFeatureFlagRepository();
        repo.Seed(CreateFlag("duplicate.key"));
        var handler = new CreateFeatureFlagCommandHandler(repo, new FixedDateTimeProvider(Now), new FakeAuditLogWriter());

        var result = await handler.HandleAsync(
            new CreateFeatureFlagCommand("duplicate.key", "Another", null, false));

        Assert.True(result.IsFailure);
        Assert.Same(FeatureManagementErrors.KeyAlreadyTaken, result.Error);
    }

    [Fact]
    public async Task Create_Should_Fail_For_Invalid_Key()
    {
        var handler = new CreateFeatureFlagCommandHandler(
            new InMemoryFeatureFlagRepository(), new FixedDateTimeProvider(Now), new FakeAuditLogWriter());

        var result = await handler.HandleAsync(
            new CreateFeatureFlagCommand("INVALID KEY!", "Name", null, false));

        Assert.True(result.IsFailure);
        Assert.Same(FeatureManagementErrors.KeyInvalid, result.Error);
    }

    [Fact]
    public async Task Create_Should_Fail_For_Empty_Name()
    {
        var handler = new CreateFeatureFlagCommandHandler(
            new InMemoryFeatureFlagRepository(), new FixedDateTimeProvider(Now), new FakeAuditLogWriter());

        var result = await handler.HandleAsync(
            new CreateFeatureFlagCommand("valid.key", "  ", null, false));

        Assert.True(result.IsFailure);
        Assert.Same(FeatureManagementErrors.NameEmpty, result.Error);
    }

    // ── UpdateFeatureFlagCommand ───────────────────────────────────────────────

    [Fact]
    public async Task Update_Should_Change_Name_And_Write_Audit()
    {
        var repo = new InMemoryFeatureFlagRepository();
        repo.Seed(CreateFlag("update.test"));
        var audit = new FakeAuditLogWriter();
        var handler = new UpdateFeatureFlagCommandHandler(repo, new FixedDateTimeProvider(Now), audit);

        var result = await handler.HandleAsync(new UpdateFeatureFlagCommand("update.test", "New Name", "New Desc"));

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Value.Name);
        Assert.Single(audit.Entries, e => e.Action == "FeatureFlagUpdated");
    }

    [Fact]
    public async Task Update_Should_Fail_When_Flag_Not_Found()
    {
        var handler = new UpdateFeatureFlagCommandHandler(
            new InMemoryFeatureFlagRepository(), new FixedDateTimeProvider(Now), new FakeAuditLogWriter());

        var result = await handler.HandleAsync(new UpdateFeatureFlagCommand("missing.flag", "Name", null));

        Assert.True(result.IsFailure);
        Assert.Same(FeatureManagementErrors.FeatureFlagNotFound, result.Error);
    }

    // ── ToggleFeatureFlagCommand ───────────────────────────────────────────────

    [Fact]
    public async Task Toggle_Should_Flip_IsEnabled_And_Write_Audit()
    {
        var repo = new InMemoryFeatureFlagRepository();
        repo.Seed(CreateFlag("toggle.test", isEnabled: false));
        var audit = new FakeAuditLogWriter();
        var handler = new ToggleFeatureFlagCommandHandler(repo, new FixedDateTimeProvider(Now), audit);

        var result = await handler.HandleAsync(new ToggleFeatureFlagCommand("toggle.test"));

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.IsEnabled);
        Assert.Single(audit.Entries, e => e.Action == "FeatureFlagToggled");
    }

    [Fact]
    public async Task Toggle_Should_Fail_When_Flag_Not_Found()
    {
        var handler = new ToggleFeatureFlagCommandHandler(
            new InMemoryFeatureFlagRepository(), new FixedDateTimeProvider(Now), new FakeAuditLogWriter());

        var result = await handler.HandleAsync(new ToggleFeatureFlagCommand("missing.flag"));

        Assert.True(result.IsFailure);
        Assert.Same(FeatureManagementErrors.FeatureFlagNotFound, result.Error);
    }

    private static FeatureFlag CreateFlag(string key = "test.flag", bool isEnabled = true) =>
        FeatureFlag.Create(
            FeatureFlagId.New().Value,
            FeatureFlagKey.Create(key).Value,
            "Test Flag",
            null,
            isEnabled,
            Now).Value;
}

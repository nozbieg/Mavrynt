using Mavrynt.Modules.FeatureManagement.Domain.Entities;
using Mavrynt.Modules.FeatureManagement.Domain.Errors;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;
using Xunit;

namespace Mavrynt.Modules.FeatureManagement.Domain.Tests;

public sealed class FeatureFlagTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_Should_Return_Flag_With_Correct_Properties()
    {
        var id = FeatureFlagId.New().Value;
        var key = FeatureFlagKey.Create("users.role-management").Value;

        var result = FeatureFlag.Create(id, key, "Users role management", "desc", true, Now);

        Assert.True(result.IsSuccess);
        Assert.Equal("users.role-management", result.Value.Key.Value);
        Assert.Equal("Users role management", result.Value.Name);
        Assert.Equal("desc", result.Value.Description);
        Assert.True(result.Value.IsEnabled);
        Assert.Equal(Now, result.Value.CreatedAt);
        Assert.Null(result.Value.UpdatedAt);
    }

    [Fact]
    public void Create_Should_Fail_When_Name_Is_Empty()
    {
        var id = FeatureFlagId.New().Value;
        var key = FeatureFlagKey.Create("some.flag").Value;

        var result = FeatureFlag.Create(id, key, "   ", null, false, Now);

        Assert.True(result.IsFailure);
        Assert.Same(FeatureManagementErrors.NameEmpty, result.Error);
    }

    [Fact]
    public void Create_Should_Fail_When_Name_Exceeds_Max_Length()
    {
        var id = FeatureFlagId.New().Value;
        var key = FeatureFlagKey.Create("some.flag").Value;

        var result = FeatureFlag.Create(id, key, new string('x', 257), null, false, Now);

        Assert.True(result.IsFailure);
        Assert.Same(FeatureManagementErrors.NameTooLong, result.Error);
    }

    // ── Key validation ────────────────────────────────────────────────────────

    [Fact]
    public void FeatureFlagKey_Should_Reject_Empty_Value()
    {
        var result = FeatureFlagKey.Create("");
        Assert.True(result.IsFailure);
        Assert.Same(FeatureManagementErrors.KeyEmpty, result.Error);
    }

    [Fact]
    public void FeatureFlagKey_Should_Reject_Null_Value()
    {
        var result = FeatureFlagKey.Create(null);
        Assert.True(result.IsFailure);
        Assert.Same(FeatureManagementErrors.KeyEmpty, result.Error);
    }

    [Theory]
    [InlineData("admin.feature-management")]
    [InlineData("users.role-management")]
    [InlineData("landing.marketing-v2")]
    [InlineData("abc")]
    [InlineData("a1.b2-c3_d4")]
    public void FeatureFlagKey_Should_Accept_Valid_Keys(string key)
    {
        var result = FeatureFlagKey.Create(key);
        Assert.True(result.IsSuccess);
        Assert.Equal(key, result.Value.Value);
    }

    [Theory]
    [InlineData("UPPERCASE")]
    [InlineData("with spaces")]
    [InlineData(".starts-with-dot")]
    [InlineData("-starts-with-dash")]
    [InlineData("contains!special")]
    [InlineData("")]
    public void FeatureFlagKey_Should_Reject_Invalid_Keys(string key)
    {
        var result = FeatureFlagKey.Create(key);
        Assert.True(result.IsFailure);
    }

    // ── Enable / Disable / Toggle ─────────────────────────────────────────────

    [Fact]
    public void Enable_Should_Set_IsEnabled_True()
    {
        var flag = CreateFlag(isEnabled: false);

        var result = flag.Enable(Now.AddMinutes(1));

        Assert.True(result.IsSuccess);
        Assert.True(flag.IsEnabled);
        Assert.NotNull(flag.UpdatedAt);
    }

    [Fact]
    public void Disable_Should_Set_IsEnabled_False()
    {
        var flag = CreateFlag(isEnabled: true);

        var result = flag.Disable(Now.AddMinutes(1));

        Assert.True(result.IsSuccess);
        Assert.False(flag.IsEnabled);
        Assert.NotNull(flag.UpdatedAt);
    }

    [Fact]
    public void Toggle_Should_Flip_IsEnabled_From_False_To_True()
    {
        var flag = CreateFlag(isEnabled: false);
        flag.Toggle(Now.AddMinutes(1));
        Assert.True(flag.IsEnabled);
    }

    [Fact]
    public void Toggle_Should_Flip_IsEnabled_From_True_To_False()
    {
        var flag = CreateFlag(isEnabled: true);
        flag.Toggle(Now.AddMinutes(1));
        Assert.False(flag.IsEnabled);
    }

    // ── UpdateDetails ──────────────────────────────────────────────────────────

    [Fact]
    public void UpdateDetails_Should_Change_Name_And_Description()
    {
        var flag = CreateFlag();

        var result = flag.UpdateDetails("New Name", "New Description", Now.AddMinutes(1));

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", flag.Name);
        Assert.Equal("New Description", flag.Description);
        Assert.NotNull(flag.UpdatedAt);
    }

    [Fact]
    public void UpdateDetails_Should_Fail_When_Name_Is_Empty()
    {
        var flag = CreateFlag();

        var result = flag.UpdateDetails("", null, Now.AddMinutes(1));

        Assert.True(result.IsFailure);
        Assert.Same(FeatureManagementErrors.NameEmpty, result.Error);
    }

    // ── FeatureFlagId ──────────────────────────────────────────────────────────

    [Fact]
    public void FeatureFlagId_Should_Reject_Empty_Guid()
    {
        var result = FeatureFlagId.From(Guid.Empty);
        Assert.True(result.IsFailure);
        Assert.Same(FeatureManagementErrors.InvalidFeatureFlagId, result.Error);
    }

    [Fact]
    public void FeatureFlagId_New_Should_Generate_Non_Empty_Id()
    {
        var result = FeatureFlagId.New();
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Value);
    }

    private static FeatureFlag CreateFlag(bool isEnabled = true) =>
        FeatureFlag.Create(
            FeatureFlagId.New().Value,
            FeatureFlagKey.Create("test.flag").Value,
            "Test Flag",
            null,
            isEnabled,
            Now).Value;
}

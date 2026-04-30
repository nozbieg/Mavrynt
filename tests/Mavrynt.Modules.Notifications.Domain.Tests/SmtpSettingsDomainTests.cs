using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Xunit;

namespace Mavrynt.Modules.Notifications.Domain.Tests;

public sealed class SmtpSettingsDomainTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    [Fact]
    public void Create_Should_Succeed_With_Valid_Values()
    {
        var result = CreateValid();

        Assert.True(result.IsSuccess);
        Assert.Equal("Test Provider", result.Value.ProviderName);
        Assert.Equal("smtp.test.com", result.Value.Host);
        Assert.Equal(587, result.Value.Port);
        Assert.Equal("user@test.com", result.Value.Username);
        Assert.Equal("sender@test.com", result.Value.SenderEmail);
        Assert.False(result.Value.IsEnabled);
    }

    [Fact]
    public void Create_Should_Fail_With_Empty_Host()
    {
        var result = SmtpSettings.Create(
            SmtpSettingsId.New().Value,
            "Provider", "", 587, "user", "pass", "sender@test.com", "Sender", false, false, Now);

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.SmtpSettingsHostEmpty, result.Error);
    }

    [Fact]
    public void Create_Should_Fail_With_Invalid_Port()
    {
        var result = SmtpSettings.Create(
            SmtpSettingsId.New().Value,
            "Provider", "host", 0, "user", "pass", "sender@test.com", "Sender", false, false, Now);

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.SmtpSettingsPortInvalid, result.Error);
    }

    [Fact]
    public void Create_Should_Fail_With_Empty_Password()
    {
        var result = SmtpSettings.Create(
            SmtpSettingsId.New().Value,
            "Provider", "host", 587, "user", "", "sender@test.com", "Sender", false, false, Now);

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.SmtpSettingsPasswordEmpty, result.Error);
    }

    [Fact]
    public void Enable_Should_Set_IsEnabled_True()
    {
        var settings = CreateValid().Value;

        settings.Enable(Now);

        Assert.True(settings.IsEnabled);
        Assert.NotNull(settings.UpdatedAt);
    }

    [Fact]
    public void Disable_Should_Set_IsEnabled_False()
    {
        var settings = CreateValid(isEnabled: true).Value;

        settings.Disable(Now);

        Assert.False(settings.IsEnabled);
    }

    [Fact]
    public void Update_Should_Change_Fields()
    {
        var settings = CreateValid().Value;

        var result = settings.Update(
            "New Provider", "new.host.com", 465, "newuser",
            "newpass", "newsender@test.com", "New Sender", true, Now.AddMinutes(1));

        Assert.True(result.IsSuccess);
        Assert.Equal("New Provider", settings.ProviderName);
        Assert.Equal("new.host.com", settings.Host);
        Assert.Equal(465, settings.Port);
        Assert.True(settings.UseSsl);
    }

    [Fact]
    public void Update_With_Null_Password_Should_Keep_Existing_Password()
    {
        var settings = CreateValid().Value;
        var originalPassword = settings.ProtectedPassword;

        settings.Update("New Provider", "new.host.com", 465, "newuser",
            null, "newsender@test.com", "New Sender", false, Now.AddMinutes(1));

        Assert.Equal(originalPassword, settings.ProtectedPassword);
    }

    private static Result<SmtpSettings> CreateValid(bool isEnabled = false) =>
        SmtpSettings.Create(
            SmtpSettingsId.New().Value,
            "Test Provider",
            "smtp.test.com",
            587,
            "user@test.com",
            "secret",
            "sender@test.com",
            "Test Sender",
            false,
            isEnabled,
            Now);
}

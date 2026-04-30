using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Xunit;

namespace Mavrynt.Modules.Notifications.Domain.Tests;

public sealed class EmailTemplateKeyTests
{
    [Theory]
    [InlineData(EmailTemplateKey.LoginConfirmation)]
    [InlineData(EmailTemplateKey.PasswordReset)]
    [InlineData(EmailTemplateKey.TwoFactorCode)]
    public void Create_Should_Succeed_For_Predefined_Keys(string key)
    {
        var result = EmailTemplateKey.Create(key);

        Assert.True(result.IsSuccess);
        Assert.Equal(key, result.Value.Value);
    }

    [Theory]
    [InlineData("unknown.key")]
    [InlineData("auth.unknown")]
    [InlineData("LOGIN_CONFIRMATION")]
    public void Create_Should_Fail_For_Unknown_Keys(string key)
    {
        var result = EmailTemplateKey.Create(key);

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.EmailTemplateKeyUnknown, result.Error);
    }

    [Fact]
    public void Create_Should_Fail_For_Empty_Key()
    {
        var result = EmailTemplateKey.Create(string.Empty);

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.EmailTemplateKeyEmpty, result.Error);
    }

    [Fact]
    public void Create_Should_Fail_For_Null_Key()
    {
        var result = EmailTemplateKey.Create(null);

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.EmailTemplateKeyEmpty, result.Error);
    }

    [Fact]
    public void Create_Should_Trim_Whitespace()
    {
        var result = EmailTemplateKey.Create($"  {EmailTemplateKey.LoginConfirmation}  ");

        Assert.True(result.IsSuccess);
        Assert.Equal(EmailTemplateKey.LoginConfirmation, result.Value.Value);
    }

    [Fact]
    public void AllKeys_Should_Contain_All_Predefined_Keys()
    {
        Assert.Contains(EmailTemplateKey.LoginConfirmation, EmailTemplateKey.AllKeys);
        Assert.Contains(EmailTemplateKey.PasswordReset, EmailTemplateKey.AllKeys);
        Assert.Contains(EmailTemplateKey.TwoFactorCode, EmailTemplateKey.AllKeys);
        Assert.Equal(3, EmailTemplateKey.AllKeys.Count);
    }
}

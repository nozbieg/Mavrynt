using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Xunit;

namespace Mavrynt.Modules.Notifications.Domain.Tests;

public sealed class EmailTemplateDomainTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    [Fact]
    public void Create_Should_Succeed_With_Valid_Values()
    {
        var key = EmailTemplateKey.Create(EmailTemplateKey.LoginConfirmation).Value;
        var result = EmailTemplate.Create(
            EmailTemplateId.New().Value,
            key,
            "Login Confirmation",
            "Description",
            "Subject {{UserEmail}}",
            "<p>Hello {{DisplayName}}</p>",
            "Hello {{DisplayName}}",
            true,
            Now);

        Assert.True(result.IsSuccess);
        Assert.Equal(EmailTemplateKey.LoginConfirmation, result.Value.Key.Value);
        Assert.Equal("Login Confirmation", result.Value.DisplayName);
        Assert.True(result.Value.IsEnabled);
    }

    [Fact]
    public void Create_Should_Fail_With_Empty_DisplayName()
    {
        var key = EmailTemplateKey.Create(EmailTemplateKey.PasswordReset).Value;
        var result = EmailTemplate.Create(
            EmailTemplateId.New().Value, key, "",
            null, "Subject", "<p>Body</p>", null, true, Now);

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.EmailTemplateDisplayNameEmpty, result.Error);
    }

    [Fact]
    public void Create_Should_Fail_With_Empty_Subject()
    {
        var key = EmailTemplateKey.Create(EmailTemplateKey.PasswordReset).Value;
        var result = EmailTemplate.Create(
            EmailTemplateId.New().Value, key, "Name",
            null, "", "<p>Body</p>", null, true, Now);

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.EmailTemplateSubjectEmpty, result.Error);
    }

    [Fact]
    public void Create_Should_Fail_With_Empty_HtmlBody()
    {
        var key = EmailTemplateKey.Create(EmailTemplateKey.TwoFactorCode).Value;
        var result = EmailTemplate.Create(
            EmailTemplateId.New().Value, key, "Name",
            null, "Subject", "", null, true, Now);

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.EmailTemplateHtmlBodyEmpty, result.Error);
    }

    [Fact]
    public void UpdateContent_Should_Change_Fields()
    {
        var template = CreateValid().Value;

        var result = template.UpdateContent(
            "New Name", "New Desc",
            "New Subject",
            "<p>New HTML</p>",
            "New Text",
            false,
            Now.AddMinutes(1));

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", template.DisplayName);
        Assert.Equal("New Subject", template.SubjectTemplate);
        Assert.False(template.IsEnabled);
        Assert.NotNull(template.UpdatedAt);
    }

    [Fact]
    public void UpdateContent_With_Null_Fields_Should_Preserve_Existing()
    {
        var template = CreateValid().Value;
        var originalSubject = template.SubjectTemplate;
        var originalHtml = template.HtmlBodyTemplate;

        template.UpdateContent(null, null, null, null, null, null, Now.AddMinutes(1));

        Assert.Equal(originalSubject, template.SubjectTemplate);
        Assert.Equal(originalHtml, template.HtmlBodyTemplate);
    }

    [Fact]
    public void Template_Key_Is_Preserved_After_Update()
    {
        var template = CreateValid().Value;
        var originalKey = template.Key.Value;

        template.UpdateContent("New Name", null, null, null, null, null, Now.AddMinutes(1));

        Assert.Equal(originalKey, template.Key.Value);
    }

    private static global::Mavrynt.BuildingBlocks.Domain.Results.Result<EmailTemplate> CreateValid() =>
        EmailTemplate.Create(
            EmailTemplateId.New().Value,
            EmailTemplateKey.Create(EmailTemplateKey.LoginConfirmation).Value,
            "Login Confirmation",
            "Description",
            "Subject",
            "<p>HTML body</p>",
            "Text body",
            true,
            Now);
}

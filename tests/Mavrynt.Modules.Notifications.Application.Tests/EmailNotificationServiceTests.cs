using Mavrynt.Modules.Notifications.Application.Models;
using Mavrynt.Modules.Notifications.Application.Services;
using Mavrynt.Modules.Notifications.Application.Tests.Fakes;
using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Mavrynt.Modules.Notifications.Application.Tests;

public sealed class EmailNotificationServiceTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    [Fact]
    public async Task SendAsync_Should_Succeed_And_Call_Sender()
    {
        var repo = new InMemoryEmailTemplateRepository();
        repo.Seed(CreateEnabledTemplate(EmailTemplateKey.LoginConfirmation));
        var sender = new FakeEmailSender();
        var service = CreateService(repo, sender);

        var model = new LoginConfirmationEmailModel("user@example.com", "User", Now, null, null);
        var result = await service.SendAsync(
            EmailTemplateKey.Create(EmailTemplateKey.LoginConfirmation).Value,
            new EmailRecipient("user@example.com"),
            model);

        Assert.True(result.IsSuccess);
        Assert.Single(sender.SentMessages);
        Assert.Equal("user@example.com", sender.SentMessages[0].Recipient.Email);
    }

    [Fact]
    public async Task SendAsync_Should_Return_NotFound_When_Template_Missing()
    {
        var repo = new InMemoryEmailTemplateRepository();
        var service = CreateService(repo, new FakeEmailSender());

        var model = new LoginConfirmationEmailModel("user@example.com", null, Now, null, null);
        var result = await service.SendAsync(
            EmailTemplateKey.Create(EmailTemplateKey.LoginConfirmation).Value,
            new EmailRecipient("user@example.com"),
            model);

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.EmailTemplateNotFound, result.Error);
    }

    [Fact]
    public async Task SendAsync_Should_Return_Disabled_When_Template_Disabled()
    {
        var repo = new InMemoryEmailTemplateRepository();
        repo.Seed(CreateDisabledTemplate(EmailTemplateKey.PasswordReset));
        var service = CreateService(repo, new FakeEmailSender());

        var model = new PasswordResetEmailModel("user@example.com", null, "https://reset", Now.AddHours(1));
        var result = await service.SendAsync(
            EmailTemplateKey.Create(EmailTemplateKey.PasswordReset).Value,
            new EmailRecipient("user@example.com"),
            model);

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.EmailTemplateDisabled, result.Error);
    }

    [Fact]
    public async Task SendAsync_Should_Propagate_Sender_Failure()
    {
        var repo = new InMemoryEmailTemplateRepository();
        repo.Seed(CreateEnabledTemplate(EmailTemplateKey.TwoFactorCode));
        var sender = new FakeEmailSender { ShouldFail = true };
        var service = CreateService(repo, sender);

        var model = new TwoFactorCodeEmailModel("user@example.com", null, "123456", Now.AddMinutes(5));
        var result = await service.SendAsync(
            EmailTemplateKey.Create(EmailTemplateKey.TwoFactorCode).Value,
            new EmailRecipient("user@example.com"),
            model);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task SendAsync_Should_Fail_For_Unknown_Placeholder_In_Template()
    {
        var repo = new InMemoryEmailTemplateRepository();
        var key = EmailTemplateKey.Create(EmailTemplateKey.LoginConfirmation).Value;
        var brokenTemplate = EmailTemplate.Create(
            EmailTemplateId.New().Value, key,
            "Test", null,
            "Hello {{UnknownPlaceholder}}",
            "<p>{{UserEmail}}</p>",
            null, true, Now).Value;
        repo.Seed(brokenTemplate);
        var service = CreateService(repo, new FakeEmailSender());

        var model = new LoginConfirmationEmailModel("user@example.com", null, Now, null, null);
        var result = await service.SendAsync(key, new EmailRecipient("user@example.com"), model);

        Assert.True(result.IsFailure);
        Assert.Equal("Notifications.Email.UnknownPlaceholder", result.Error.Code);
    }

    private static EmailNotificationService CreateService(
        InMemoryEmailTemplateRepository repo, FakeEmailSender sender) =>
        new(repo, new EmailTemplateRenderer(), sender, NullLogger<EmailNotificationService>.Instance);

    private static EmailTemplate CreateEnabledTemplate(string keyStr) =>
        EmailTemplate.Create(
            EmailTemplateId.New().Value,
            EmailTemplateKey.Create(keyStr).Value,
            "Test Template",
            null,
            "Subject {{UserEmail}}",
            "<p>Hello {{DisplayName}} {{UserEmail}} {{LoginAt}} {{IpAddress}} {{UserAgent}}</p>",
            "Hello {{DisplayName}} {{UserEmail}} {{LoginAt}} {{IpAddress}} {{UserAgent}}",
            true,
            Now).Value;

    private static EmailTemplate CreateDisabledTemplate(string keyStr)
    {
        var key = EmailTemplateKey.Create(keyStr).Value;
        return EmailTemplate.Create(
            EmailTemplateId.New().Value, key,
            "Disabled", null,
            "Subject {{UserEmail}}",
            "<p>{{DisplayName}} {{UserEmail}} {{ResetLink}} {{ExpiresAt}}</p>",
            null, false, Now).Value;
    }
}

using Mavrynt.Modules.Notifications.Application.Commands;
using Mavrynt.Modules.Notifications.Application.DTOs;
using Mavrynt.Modules.Notifications.Application.Tests.Fakes;
using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Xunit;

namespace Mavrynt.Modules.Notifications.Application.Tests;

public sealed class CommandHandlersTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    // ── CreateSmtpSettings ─────────────────────────────────────────────────────

    [Fact]
    public async Task CreateSmtp_Should_Persist_And_Return_Dto_Without_Password()
    {
        var repo = new InMemorySmtpSettingsRepository();
        var audit = new FakeAuditLogWriter();
        var handler = new CreateSmtpSettingsCommandHandler(
            repo, new FixedDateTimeProvider(Now), audit, new FakeSecretProtector());

        var result = await handler.HandleAsync(new CreateSmtpSettingsCommand(
            "Provider", "smtp.host.com", 587, "user", "secret",
            "sender@test.com", "Sender", false, false));

        Assert.True(result.IsSuccess);
        Assert.Single(repo.Settings);
        Assert.Single(audit.Entries, e => e.Action == "SmtpSettingsCreated");

        // DTO must not expose password
        var dto = result.Value;
        var props = typeof(SmtpSettingsDto).GetProperties().Select(p => p.Name);
        Assert.DoesNotContain("Password", props);
        Assert.DoesNotContain("ProtectedPassword", props);
    }

    [Fact]
    public async Task CreateSmtp_Should_Protect_Password()
    {
        var repo = new InMemorySmtpSettingsRepository();
        var handler = new CreateSmtpSettingsCommandHandler(
            repo, new FixedDateTimeProvider(Now), new FakeAuditLogWriter(), new FakeSecretProtector());

        await handler.HandleAsync(new CreateSmtpSettingsCommand(
            "Provider", "smtp.host.com", 587, "user", "mypassword",
            "sender@test.com", "Sender", false, false));

        Assert.Equal("protected:mypassword", repo.Settings[0].ProtectedPassword);
    }

    [Fact]
    public async Task CreateSmtp_Enabled_Should_Disable_Others_First()
    {
        var repo = new InMemorySmtpSettingsRepository();
        var existing = SmtpSettings.Create(
            SmtpSettingsId.New().Value, "Old", "old.host", 587, "u", "p",
            "old@test.com", "Old", false, true, Now).Value;
        repo.Seed(existing);

        var handler = new CreateSmtpSettingsCommandHandler(
            repo, new FixedDateTimeProvider(Now), new FakeAuditLogWriter(), new FakeSecretProtector());

        await handler.HandleAsync(new CreateSmtpSettingsCommand(
            "New Provider", "new.host.com", 587, "user", "secret",
            "new@test.com", "Sender", false, true));

        Assert.False(existing.IsEnabled);
        Assert.True(repo.Settings[1].IsEnabled);
    }

    // ── UpdateEmailTemplate ────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateEmailTemplate_Should_Update_Fields()
    {
        var repo = new InMemoryEmailTemplateRepository();
        repo.Seed(CreateTemplate(EmailTemplateKey.LoginConfirmation));
        var audit = new FakeAuditLogWriter();
        var handler = new UpdateEmailTemplateCommandHandler(
            repo, new FixedDateTimeProvider(Now), audit);

        var result = await handler.HandleAsync(new UpdateEmailTemplateCommand(
            EmailTemplateKey.LoginConfirmation, "New Name", null, "New Subject",
            "<p>{{UserEmail}} {{DisplayName}} {{LoginAt}} {{IpAddress}} {{UserAgent}}</p>",
            null, false));

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Value.DisplayName);
        Assert.False(result.Value.IsEnabled);
        Assert.Single(audit.Entries, e => e.Action == "EmailTemplateUpdated");
    }

    [Fact]
    public async Task UpdateEmailTemplate_Should_Fail_For_Unknown_Key()
    {
        var handler = new UpdateEmailTemplateCommandHandler(
            new InMemoryEmailTemplateRepository(), new FixedDateTimeProvider(Now), new FakeAuditLogWriter());

        var result = await handler.HandleAsync(new UpdateEmailTemplateCommand(
            "auth.unknown_key", null, null, null, null, null, null));

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.EmailTemplateKeyUnknown, result.Error);
    }

    [Fact]
    public async Task UpdateEmailTemplate_Should_Fail_For_Missing_Template()
    {
        var handler = new UpdateEmailTemplateCommandHandler(
            new InMemoryEmailTemplateRepository(), new FixedDateTimeProvider(Now), new FakeAuditLogWriter());

        var result = await handler.HandleAsync(new UpdateEmailTemplateCommand(
            EmailTemplateKey.PasswordReset, null, null, null, null, null, null));

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.EmailTemplateNotFound, result.Error);
    }

    // ── EnableSmtpSettings ─────────────────────────────────────────────────────

    [Fact]
    public async Task EnableSmtp_Should_Enable_And_Disable_Others()
    {
        var repo = new InMemorySmtpSettingsRepository();
        var activeOther = SmtpSettings.Create(
            SmtpSettingsId.New().Value, "Other", "other.host", 587, "u", "p",
            "other@test.com", "Other", false, true, Now).Value;
        repo.Seed(activeOther);
        var target = SmtpSettings.Create(
            SmtpSettingsId.New().Value, "Target", "target.host", 587, "u", "p",
            "target@test.com", "Target", false, false, Now).Value;
        repo.Seed(target);

        var handler = new EnableSmtpSettingsCommandHandler(
            repo, new FixedDateTimeProvider(Now), new FakeAuditLogWriter());

        var result = await handler.HandleAsync(new EnableSmtpSettingsCommand(target.Id.Value));

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.IsEnabled);
        Assert.False(activeOther.IsEnabled);
    }

    [Fact]
    public async Task EnableSmtp_Should_Fail_When_Not_Found()
    {
        var handler = new EnableSmtpSettingsCommandHandler(
            new InMemorySmtpSettingsRepository(), new FixedDateTimeProvider(Now), new FakeAuditLogWriter());

        var result = await handler.HandleAsync(new EnableSmtpSettingsCommand(Guid.NewGuid()));

        Assert.True(result.IsFailure);
        Assert.Same(NotificationsErrors.SmtpSettingsNotFound, result.Error);
    }

    private static EmailTemplate CreateTemplate(string keyStr) =>
        EmailTemplate.Create(
            EmailTemplateId.New().Value,
            EmailTemplateKey.Create(keyStr).Value,
            "Login Confirmation",
            null,
            "Subject {{UserEmail}}",
            "<p>{{DisplayName}} {{UserEmail}} {{LoginAt}} {{IpAddress}} {{UserAgent}}</p>",
            null, true, Now).Value;
}

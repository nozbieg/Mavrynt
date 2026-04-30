using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Audit.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.Models;
using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Application.Tests.Fakes;

internal sealed class FixedDateTimeProvider(DateTimeOffset utcNow) : IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; } = utcNow;
}

internal sealed class FakeAuditLogWriter : IAuditLogWriter
{
    public List<(Guid? ActorUserId, string Action, string ResourceType, string? ResourceId)> Entries { get; } = [];

    public Task WriteAsync(
        Guid? actorUserId, string action, string resourceType,
        string? resourceId, string? metadataJson = null, CancellationToken cancellationToken = default)
    {
        Entries.Add((actorUserId, action, resourceType, resourceId));
        return Task.CompletedTask;
    }
}

internal sealed class FakeSecretProtector : ISecretProtector
{
    public string Protect(string plaintext) => $"protected:{plaintext}";
    public string Unprotect(string protectedValue) =>
        protectedValue.StartsWith("protected:") ? protectedValue["protected:".Length..] : protectedValue;
}

internal sealed class FakeEmailSender : IEmailSender
{
    public List<EmailMessage> SentMessages { get; } = [];
    public bool ShouldFail { get; set; }

    public Task<Result> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (ShouldFail)
            return Task.FromResult(Result.Failure(new Error("Fake.SendFailed", "Fake send failed.")));
        SentMessages.Add(message);
        return Task.FromResult(Result.Success());
    }
}

internal sealed class InMemorySmtpSettingsRepository : ISmtpSettingsRepository
{
    public List<SmtpSettings> Settings { get; } = [];

    public void Seed(SmtpSettings settings) => Settings.Add(settings);

    public Task<SmtpSettings?> GetByIdAsync(SmtpSettingsId id, CancellationToken cancellationToken = default)
        => Task.FromResult(Settings.FirstOrDefault(s => s.Id == id));

    public Task<SmtpSettings?> GetActiveAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Settings.FirstOrDefault(s => s.IsEnabled));

    public Task<IReadOnlyList<SmtpSettings>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<SmtpSettings>>(Settings.AsReadOnly());

    public Task AddAsync(SmtpSettings settings, CancellationToken cancellationToken = default)
    {
        Settings.Add(settings);
        return Task.CompletedTask;
    }

    public Task DisableAllAsync(CancellationToken cancellationToken = default)
    {
        foreach (var s in Settings.Where(s => s.IsEnabled))
            s.Disable(DateTimeOffset.UtcNow);
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryEmailTemplateRepository : IEmailTemplateRepository
{
    public List<EmailTemplate> Templates { get; } = [];

    public void Seed(EmailTemplate template) => Templates.Add(template);

    public Task<EmailTemplate?> GetByIdAsync(EmailTemplateId id, CancellationToken cancellationToken = default)
        => Task.FromResult(Templates.FirstOrDefault(t => t.Id == id));

    public Task<EmailTemplate?> GetByKeyAsync(EmailTemplateKey key, CancellationToken cancellationToken = default)
        => Task.FromResult(Templates.FirstOrDefault(t => t.Key == key));

    public Task<IReadOnlyList<EmailTemplate>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<EmailTemplate>>(Templates.AsReadOnly());

    public Task<bool> ExistsByKeyAsync(EmailTemplateKey key, CancellationToken cancellationToken = default)
        => Task.FromResult(Templates.Any(t => t.Key == key));

    public Task AddAsync(EmailTemplate template, CancellationToken cancellationToken = default)
    {
        Templates.Add(template);
        return Task.CompletedTask;
    }
}

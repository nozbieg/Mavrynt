using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Mavrynt.Modules.Notifications.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Mavrynt.Modules.Notifications.Infrastructure.Seeding;

internal sealed class DefaultSmtpSettingsSeeder
{
    private const string DefaultProviderName = "Local Dev SMTP";
    private const string DefaultHost = "localhost";
    private const int DefaultPort = 1025;
    private const string DefaultUsername = "dev";
    private const string DefaultPassword = "dev";
    private const string DefaultSenderEmail = "noreply@mavrynt.local";
    private const string DefaultSenderName = "Mavrynt";

    private readonly ISmtpSettingsRepository _repository;
    private readonly NotificationsDbContext _context;
    private readonly ISecretProtector _secretProtector;
    private readonly ILogger<DefaultSmtpSettingsSeeder> _logger;

    public DefaultSmtpSettingsSeeder(
        ISmtpSettingsRepository repository,
        NotificationsDbContext context,
        ISecretProtector secretProtector,
        ILogger<DefaultSmtpSettingsSeeder> logger)
    {
        _repository = repository;
        _context = context;
        _secretProtector = secretProtector;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _repository.AnyAsync(cancellationToken))
        {
            _logger.LogDebug("SMTP settings already exist, skipping default SMTP seed.");
            return;
        }

        var idResult = SmtpSettingsId.New();
        if (idResult.IsFailure)
        {
            _logger.LogError("Failed to generate default SMTP settings id: {ErrorCode}", idResult.Error.Code);
            return;
        }

        var protectedPassword = _secretProtector.Protect(DefaultPassword);

        var settingsResult = SmtpSettings.Create(
            idResult.Value,
            DefaultProviderName,
            DefaultHost,
            DefaultPort,
            DefaultUsername,
            protectedPassword,
            DefaultSenderEmail,
            DefaultSenderName,
            useSsl: false,
            isEnabled: true,
            createdAt: DateTimeOffset.UtcNow);

        if (settingsResult.IsFailure)
        {
            _logger.LogError(
                "Failed to build default SMTP settings: {ErrorCode}",
                settingsResult.Error.Code);
            return;
        }

        await _repository.AddAsync(settingsResult.Value, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Seeded default local SMTP configuration {Provider} ({Host}:{Port}).",
            DefaultProviderName, DefaultHost, DefaultPort);
    }
}

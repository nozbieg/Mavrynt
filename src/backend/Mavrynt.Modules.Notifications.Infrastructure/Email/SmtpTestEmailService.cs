using System.Net;
using System.Net.Mail;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Mavrynt.Modules.Notifications.Infrastructure.Email;

internal sealed class SmtpTestEmailService : ISmtpTestEmailService
{
    private const string TestSubject = "Mavrynt SMTP test email";

    private const string TestHtmlBody =
        "<p>This is a test email from Mavrynt AdminApp.</p>" +
        "<p>If you received this message, the selected SMTP configuration works.</p>";

    private const string TestTextBody =
        "This is a test email from Mavrynt AdminApp. " +
        "If you received this message, the selected SMTP configuration works.";

    private readonly ISmtpSettingsRepository _settingsRepository;
    private readonly ISecretProtector _secretProtector;
    private readonly ILogger<SmtpTestEmailService> _logger;

    public SmtpTestEmailService(
        ISmtpSettingsRepository settingsRepository,
        ISecretProtector secretProtector,
        ILogger<SmtpTestEmailService> logger)
    {
        _settingsRepository = settingsRepository;
        _secretProtector = secretProtector;
        _logger = logger;
    }

    public async Task<Result> SendTestEmailAsync(
        Guid smtpSettingsId,
        string recipientEmail,
        CancellationToken cancellationToken = default)
    {
        var idResult = SmtpSettingsId.From(smtpSettingsId);
        if (idResult.IsFailure) return idResult.Error;

        var settings = await _settingsRepository.GetByIdAsync(idResult.Value, cancellationToken);
        if (settings is null) return NotificationsErrors.SmtpSettingsNotFound;

        var password = _secretProtector.Unprotect(settings.ProtectedPassword);

        try
        {
            using var client = new SmtpClient(settings.Host, settings.Port)
            {
                EnableSsl = settings.UseSsl,
                Credentials = new NetworkCredential(settings.Username, password),
            };

            var from = new MailAddress(settings.SenderEmail, settings.SenderName);
            var to = new MailAddress(recipientEmail);

            using var mailMessage = new MailMessage(from, to)
            {
                Subject = TestSubject,
                Body = TestHtmlBody,
                IsBodyHtml = true,
            };

            var textView = AlternateView.CreateAlternateViewFromString(
                TestTextBody, null, "text/plain");
            mailMessage.AlternateViews.Add(textView);

            await client.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInformation(
                "SMTP test email sent via {Provider} to recipient domain {RecipientDomain}",
                settings.ProviderName,
                GetDomain(recipientEmail));

            return Result.Success();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex,
                "SMTP test email failed via provider {Provider}",
                settings.ProviderName);
            return NotificationsErrors.EmailSendFailed;
        }
    }

    private static string GetDomain(string email)
    {
        var atIndex = email.IndexOf('@');
        return atIndex >= 0 ? email[(atIndex + 1)..] : "unknown";
    }
}

using System.Net;
using System.Net.Mail;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.Models;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mavrynt.Modules.Notifications.Infrastructure.Email;

internal sealed class SmtpEmailSender : IEmailSender
{
    private readonly ISmtpSettingsRepository _settingsRepository;
    private readonly ISecretProtector _secretProtector;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(
        ISmtpSettingsRepository settingsRepository,
        ISecretProtector secretProtector,
        ILogger<SmtpEmailSender> logger)
    {
        _settingsRepository = settingsRepository;
        _secretProtector = secretProtector;
        _logger = logger;
    }

    public async Task<Result> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var activeSettings = await _settingsRepository.GetActiveAsync(cancellationToken);
        if (activeSettings is null)
            return NotificationsErrors.SmtpSettingsNoActiveProvider;

        var password = _secretProtector.Unprotect(activeSettings.ProtectedPassword);

        try
        {
            using var client = new SmtpClient(activeSettings.Host, activeSettings.Port)
            {
                EnableSsl = activeSettings.UseSsl,
                Credentials = new NetworkCredential(activeSettings.Username, password),
            };

            var from = new MailAddress(activeSettings.SenderEmail, activeSettings.SenderName);
            var to = new MailAddress(message.Recipient.Email, message.Recipient.DisplayName);

            using var mailMessage = new MailMessage(from, to)
            {
                Subject = message.Subject,
                Body = message.HtmlBody,
                IsBodyHtml = true,
            };

            if (!string.IsNullOrWhiteSpace(message.TextBody))
            {
                var textView = AlternateView.CreateAlternateViewFromString(
                    message.TextBody, null, "text/plain");
                mailMessage.AlternateViews.Add(textView);
            }

            await client.SendMailAsync(mailMessage, cancellationToken);
            _logger.LogInformation("Email sent via {Provider} to recipient domain {Domain}",
                activeSettings.ProviderName,
                GetDomain(message.Recipient.Email));

            return Result.Success();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "SMTP send failed via provider {Provider}", activeSettings.ProviderName);
            return NotificationsErrors.EmailSendFailed;
        }
    }

    private static string GetDomain(string email)
    {
        var atIndex = email.IndexOf('@');
        return atIndex >= 0 ? email[(atIndex + 1)..] : "unknown";
    }
}

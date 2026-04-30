using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.Models;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Mavrynt.Modules.Notifications.Application.Services;

public sealed class EmailNotificationService : IEmailNotificationService
{
    private readonly IEmailTemplateRepository _templateRepository;
    private readonly IEmailTemplateRenderer _renderer;
    private readonly IEmailSender _sender;
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(
        IEmailTemplateRepository templateRepository,
        IEmailTemplateRenderer renderer,
        IEmailSender sender,
        ILogger<EmailNotificationService> logger)
    {
        _templateRepository = templateRepository;
        _renderer = renderer;
        _sender = sender;
        _logger = logger;
    }

    public async Task<Result> SendAsync<TModel>(
        EmailTemplateKey templateKey,
        EmailRecipient recipient,
        TModel model,
        CancellationToken cancellationToken = default)
        where TModel : IEmailModel
    {
        var template = await _templateRepository.GetByKeyAsync(templateKey, cancellationToken);
        if (template is null)
            return NotificationsErrors.EmailTemplateNotFound;

        if (!template.IsEnabled)
            return NotificationsErrors.EmailTemplateDisabled;

        var placeholders = model.ToPlaceholders();
        var renderResult = _renderer.Render(
            template.SubjectTemplate,
            template.HtmlBodyTemplate,
            template.TextBodyTemplate,
            placeholders);

        if (renderResult.IsFailure)
            return renderResult.Error;

        var rendered = renderResult.Value;
        var message = new EmailMessage(recipient, rendered.Subject, rendered.HtmlBody, rendered.TextBody);

        _logger.LogInformation(
            "Sending email template {TemplateKey} to recipient domain {RecipientDomain}",
            templateKey.Value,
            GetDomain(recipient.Email));

        var sendResult = await _sender.SendAsync(message, cancellationToken);

        if (sendResult.IsFailure)
        {
            _logger.LogWarning(
                "Failed to send email template {TemplateKey}: {ErrorCode}",
                templateKey.Value, sendResult.Error.Code);
        }

        return sendResult;
    }

    private static string GetDomain(string email)
    {
        var atIndex = email.IndexOf('@');
        return atIndex >= 0 ? email[(atIndex + 1)..] : "unknown";
    }
}

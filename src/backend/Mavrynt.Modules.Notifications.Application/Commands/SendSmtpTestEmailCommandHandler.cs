using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Audit.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Domain.Errors;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed class SendSmtpTestEmailCommandHandler : ICommandHandler<SendSmtpTestEmailCommand>
{
    private readonly ISmtpTestEmailService _smtpTestEmailService;
    private readonly IAuditLogWriter _auditLogWriter;

    public SendSmtpTestEmailCommandHandler(
        ISmtpTestEmailService smtpTestEmailService,
        IAuditLogWriter auditLogWriter)
    {
        _smtpTestEmailService = smtpTestEmailService;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<Result> HandleAsync(
        SendSmtpTestEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        var recipientError = ValidateRecipient(command.RecipientEmail);
        if (recipientError is not null)
            return recipientError;

        var sendResult = await _smtpTestEmailService.SendTestEmailAsync(
            command.SmtpSettingsId,
            command.RecipientEmail.Trim(),
            cancellationToken);

        if (sendResult.IsFailure)
            return sendResult;

        await _auditLogWriter.WriteAsync(
            actorUserId: null,
            action: "SmtpTestEmailSent",
            resourceType: "SmtpSettings",
            resourceId: command.SmtpSettingsId.ToString(),
            metadataJson: null,
            cancellationToken: cancellationToken);

        return Result.Success();
    }

    private static Error? ValidateRecipient(string? recipientEmail)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
            return NotificationsErrors.EmailRecipientInvalid;

        var trimmed = recipientEmail.Trim();
        var atIndex = trimmed.IndexOf('@');
        if (atIndex <= 0 || atIndex >= trimmed.Length - 1)
            return NotificationsErrors.EmailRecipientInvalid;

        var domain = trimmed[(atIndex + 1)..];
        return domain.Contains('.') ? null : NotificationsErrors.EmailRecipientInvalid;
    }
}

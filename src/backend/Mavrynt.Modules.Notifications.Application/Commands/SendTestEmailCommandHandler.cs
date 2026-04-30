using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.Models;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed class SendTestEmailCommandHandler : ICommandHandler<SendTestEmailCommand>
{
    private readonly IEmailNotificationService _emailNotificationService;

    public SendTestEmailCommandHandler(IEmailNotificationService emailNotificationService)
    {
        _emailNotificationService = emailNotificationService;
    }

    public async Task<Result> HandleAsync(SendTestEmailCommand command, CancellationToken cancellationToken = default)
    {
        var templateKeyStr = command.TemplateKey ?? EmailTemplateKey.LoginConfirmation;
        var keyResult = EmailTemplateKey.Create(templateKeyStr);
        if (keyResult.IsFailure) return keyResult.Error;

        var recipient = new EmailRecipient(command.RecipientEmail, "Test Recipient");
        var model = new LoginConfirmationEmailModel(
            UserEmail: command.RecipientEmail,
            DisplayName: "Test User",
            LoginAt: DateTimeOffset.UtcNow,
            IpAddress: "127.0.0.1",
            UserAgent: "Mavrynt Test Client");

        return await _emailNotificationService.SendAsync(keyResult.Value, recipient, model, cancellationToken);
    }
}

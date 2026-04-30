namespace Mavrynt.Modules.Notifications.Application.Models;

public sealed record EmailMessage(
    EmailRecipient Recipient,
    string Subject,
    string HtmlBody,
    string? TextBody = null);

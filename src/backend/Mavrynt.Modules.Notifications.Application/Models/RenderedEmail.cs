namespace Mavrynt.Modules.Notifications.Application.Models;

public sealed record RenderedEmail(string Subject, string HtmlBody, string? TextBody);

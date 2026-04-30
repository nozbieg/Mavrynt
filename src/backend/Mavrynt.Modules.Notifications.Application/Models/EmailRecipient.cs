namespace Mavrynt.Modules.Notifications.Application.Models;

public sealed record EmailRecipient(string Email, string? DisplayName = null);

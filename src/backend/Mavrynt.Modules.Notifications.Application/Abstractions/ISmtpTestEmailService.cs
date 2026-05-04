using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.Modules.Notifications.Application.Abstractions;

/// <summary>
/// Sends a built-in test email through a specifically selected SMTP configuration,
/// regardless of whether it is currently active. Used by the AdminApp to verify
/// SMTP settings before activating them.
/// </summary>
public interface ISmtpTestEmailService
{
    Task<Result> SendTestEmailAsync(
        Guid smtpSettingsId,
        string recipientEmail,
        CancellationToken cancellationToken = default);
}

using Mavrynt.BuildingBlocks.Application.Messaging;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed record SendSmtpTestEmailCommand(
    Guid SmtpSettingsId,
    string RecipientEmail
) : ICommand;

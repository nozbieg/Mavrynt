using Mavrynt.BuildingBlocks.Application.Messaging;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed record SendTestEmailCommand(
    string RecipientEmail,
    string? TemplateKey
) : ICommand;

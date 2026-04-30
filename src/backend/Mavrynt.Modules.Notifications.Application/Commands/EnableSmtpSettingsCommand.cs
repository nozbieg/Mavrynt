using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Notifications.Application.DTOs;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed record EnableSmtpSettingsCommand(Guid Id) : ICommand<SmtpSettingsDto>;

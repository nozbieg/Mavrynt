using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Notifications.Application.DTOs;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed record CreateSmtpSettingsCommand(
    string ProviderName,
    string Host,
    int Port,
    string Username,
    string Password,
    string SenderEmail,
    string SenderName,
    bool UseSsl,
    bool IsEnabled
) : ICommand<SmtpSettingsDto>, ITransactionalRequest;

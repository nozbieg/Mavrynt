using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Notifications.Application.DTOs;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed record UpdateSmtpSettingsCommand(
    Guid Id,
    string ProviderName,
    string Host,
    int Port,
    string Username,
    string? Password,
    string SenderEmail,
    string SenderName,
    bool UseSsl
) : ICommand<SmtpSettingsDto>, ITransactionalRequest;

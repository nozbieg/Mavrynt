using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.Notifications.Application.DTOs;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed record UpdateEmailTemplateCommand(
    string TemplateKey,
    string? DisplayName,
    string? Description,
    string? SubjectTemplate,
    string? HtmlBodyTemplate,
    string? TextBodyTemplate,
    bool? IsEnabled
) : ICommand<EmailTemplateDto>;

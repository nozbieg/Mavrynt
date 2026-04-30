namespace Mavrynt.Modules.Notifications.Application.DTOs;

public sealed record EmailTemplateDefinitionDto(
    string TemplateKey,
    string DisplayName,
    string Description,
    IReadOnlyList<string> SupportedPlaceholders
);

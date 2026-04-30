namespace Mavrynt.Modules.Notifications.Application.DTOs;

public sealed record EmailTemplateDto(
    Guid Id,
    string TemplateKey,
    string DisplayName,
    string? Description,
    string SubjectTemplate,
    string HtmlBodyTemplate,
    string? TextBodyTemplate,
    bool IsEnabled,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);

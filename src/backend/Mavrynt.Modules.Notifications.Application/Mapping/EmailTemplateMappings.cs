using Mavrynt.Modules.Notifications.Application.DTOs;
using Mavrynt.Modules.Notifications.Domain.Entities;

namespace Mavrynt.Modules.Notifications.Application.Mapping;

internal static class EmailTemplateMappings
{
    internal static EmailTemplateDto ToDto(this EmailTemplate template) =>
        new(
            template.Id.Value,
            template.Key.Value,
            template.DisplayName,
            template.Description,
            template.SubjectTemplate,
            template.HtmlBodyTemplate,
            template.TextBodyTemplate,
            template.IsEnabled,
            template.CreatedAt,
            template.UpdatedAt
        );
}

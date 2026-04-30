using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.Models;

namespace Mavrynt.Modules.Notifications.Application.Abstractions;

public interface IEmailTemplateRenderer
{
    Result<RenderedEmail> Render(
        string subjectTemplate,
        string htmlBodyTemplate,
        string? textBodyTemplate,
        IReadOnlyDictionary<string, string> placeholders);
}

using System.Net;
using System.Text.RegularExpressions;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.Models;
using Mavrynt.Modules.Notifications.Domain.Errors;

namespace Mavrynt.Modules.Notifications.Application.Services;

public sealed class EmailTemplateRenderer : IEmailTemplateRenderer
{
    private static readonly Regex PlaceholderPattern =
        new(@"\{\{(\w+)\}\}", RegexOptions.Compiled);

    public Result<RenderedEmail> Render(
        string subjectTemplate,
        string htmlBodyTemplate,
        string? textBodyTemplate,
        IReadOnlyDictionary<string, string> placeholders)
    {
        var subjectResult = RenderPlainText(subjectTemplate, placeholders);
        if (subjectResult.IsFailure) return subjectResult.Error;

        var htmlResult = RenderHtml(htmlBodyTemplate, placeholders);
        if (htmlResult.IsFailure) return htmlResult.Error;

        string? textBody = null;
        if (textBodyTemplate is not null)
        {
            var textResult = RenderPlainText(textBodyTemplate, placeholders);
            if (textResult.IsFailure) return textResult.Error;
            textBody = textResult.Value;
        }

        return new RenderedEmail(subjectResult.Value, htmlResult.Value, textBody);
    }

    private static Result<string> RenderPlainText(
        string template,
        IReadOnlyDictionary<string, string> placeholders)
    {
        var unknowns = FindUnknownPlaceholders(template, placeholders);
        if (unknowns.Count > 0)
            return NotificationsErrors.EmailUnknownPlaceholder(string.Join(", ", unknowns));

        var result = PlaceholderPattern.Replace(template, m => placeholders[m.Groups[1].Value]);
        return result;
    }

    private static Result<string> RenderHtml(
        string template,
        IReadOnlyDictionary<string, string> placeholders)
    {
        var unknowns = FindUnknownPlaceholders(template, placeholders);
        if (unknowns.Count > 0)
            return NotificationsErrors.EmailUnknownPlaceholder(string.Join(", ", unknowns));

        var result = PlaceholderPattern.Replace(
            template,
            m => WebUtility.HtmlEncode(placeholders[m.Groups[1].Value]));
        return result;
    }

    private static List<string> FindUnknownPlaceholders(
        string template,
        IReadOnlyDictionary<string, string> placeholders)
    {
        return PlaceholderPattern.Matches(template)
            .Select(m => m.Groups[1].Value)
            .Distinct()
            .Where(name => !placeholders.ContainsKey(name))
            .ToList();
    }
}

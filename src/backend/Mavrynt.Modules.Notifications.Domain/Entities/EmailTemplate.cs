using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Domain.Entities;

public sealed class EmailTemplate : AggregateRoot<EmailTemplateId>
{
    private EmailTemplate() : base(null!) { }

    private EmailTemplate(
        EmailTemplateId id,
        EmailTemplateKey key,
        string displayName,
        string? description,
        string subjectTemplate,
        string htmlBodyTemplate,
        string? textBodyTemplate,
        bool isEnabled,
        DateTimeOffset createdAt)
        : base(id)
    {
        Key = key;
        DisplayName = displayName;
        Description = description;
        SubjectTemplate = subjectTemplate;
        HtmlBodyTemplate = htmlBodyTemplate;
        TextBodyTemplate = textBodyTemplate;
        IsEnabled = isEnabled;
        CreatedAt = createdAt;
    }

    public EmailTemplateKey Key { get; private set; } = null!;
    public string DisplayName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string SubjectTemplate { get; private set; } = string.Empty;
    public string HtmlBodyTemplate { get; private set; } = string.Empty;
    public string? TextBodyTemplate { get; private set; }
    public bool IsEnabled { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Result<EmailTemplate> Create(
        EmailTemplateId id,
        EmailTemplateKey key,
        string displayName,
        string? description,
        string subjectTemplate,
        string htmlBodyTemplate,
        string? textBodyTemplate,
        bool isEnabled,
        DateTimeOffset createdAt)
    {
        var validationError = ValidateContent(displayName, subjectTemplate, htmlBodyTemplate);
        if (validationError is not null)
            return validationError;

        return new EmailTemplate(
            id, key, displayName.Trim(), description?.Trim(),
            subjectTemplate.Trim(), htmlBodyTemplate.Trim(),
            textBodyTemplate?.Trim(), isEnabled, createdAt);
    }

    public Result UpdateContent(
        string? displayName,
        string? description,
        string? subjectTemplate,
        string? htmlBodyTemplate,
        string? textBodyTemplate,
        bool? isEnabled,
        DateTimeOffset updatedAt)
    {
        var newDisplayName = displayName?.Trim() ?? DisplayName;
        var newSubject = subjectTemplate?.Trim() ?? SubjectTemplate;
        var newHtml = htmlBodyTemplate?.Trim() ?? HtmlBodyTemplate;

        var validationError = ValidateContent(newDisplayName, newSubject, newHtml);
        if (validationError is not null)
            return validationError;

        DisplayName = newDisplayName;
        Description = description is not null ? description.Trim() : Description;
        SubjectTemplate = newSubject;
        HtmlBodyTemplate = newHtml;
        if (textBodyTemplate is not null)
            TextBodyTemplate = textBodyTemplate.Trim();
        if (isEnabled.HasValue)
            IsEnabled = isEnabled.Value;
        UpdatedAt = updatedAt;

        return Result.Success();
    }

    private static Error? ValidateContent(string displayName, string subjectTemplate, string htmlBodyTemplate)
    {
        if (string.IsNullOrWhiteSpace(displayName)) return NotificationsErrors.EmailTemplateDisplayNameEmpty;
        if (string.IsNullOrWhiteSpace(subjectTemplate)) return NotificationsErrors.EmailTemplateSubjectEmpty;
        if (string.IsNullOrWhiteSpace(htmlBodyTemplate)) return NotificationsErrors.EmailTemplateHtmlBodyEmpty;
        return null;
    }
}

using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Audit.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.DTOs;
using Mavrynt.Modules.Notifications.Application.Mapping;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed class UpdateEmailTemplateCommandHandler : ICommandHandler<UpdateEmailTemplateCommand, EmailTemplateDto>
{
    private readonly IEmailTemplateRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter;

    public UpdateEmailTemplateCommandHandler(
        IEmailTemplateRepository repository,
        IDateTimeProvider dateTimeProvider,
        IAuditLogWriter auditLogWriter)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<Result<EmailTemplateDto>> HandleAsync(
        UpdateEmailTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var keyResult = EmailTemplateKey.Create(command.TemplateKey);
        if (keyResult.IsFailure) return keyResult.Error;

        var template = await _repository.GetByKeyAsync(keyResult.Value, cancellationToken);
        if (template is null) return NotificationsErrors.EmailTemplateNotFound;

        var updateResult = template.UpdateContent(
            command.DisplayName,
            command.Description,
            command.SubjectTemplate,
            command.HtmlBodyTemplate,
            command.TextBodyTemplate,
            command.IsEnabled,
            _dateTimeProvider.UtcNow);

        if (updateResult.IsFailure) return updateResult.Error;

        await _auditLogWriter.WriteAsync(
            actorUserId: null,
            action: "EmailTemplateUpdated",
            resourceType: "EmailTemplate",
            resourceId: keyResult.Value.Value,
            metadataJson: null,
            cancellationToken: cancellationToken);

        return template.ToDto();
    }
}

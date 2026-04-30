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

public sealed class EnableSmtpSettingsCommandHandler : ICommandHandler<EnableSmtpSettingsCommand, SmtpSettingsDto>
{
    private readonly ISmtpSettingsRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter;

    public EnableSmtpSettingsCommandHandler(
        ISmtpSettingsRepository repository,
        IDateTimeProvider dateTimeProvider,
        IAuditLogWriter auditLogWriter)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<Result<SmtpSettingsDto>> HandleAsync(
        EnableSmtpSettingsCommand command,
        CancellationToken cancellationToken = default)
    {
        var idResult = SmtpSettingsId.From(command.Id);
        if (idResult.IsFailure) return idResult.Error;

        var settings = await _repository.GetByIdAsync(idResult.Value, cancellationToken);
        if (settings is null) return NotificationsErrors.SmtpSettingsNotFound;

        await _repository.DisableAllAsync(cancellationToken);

        settings.Enable(_dateTimeProvider.UtcNow);

        await _auditLogWriter.WriteAsync(
            actorUserId: null,
            action: "SmtpSettingsEnabled",
            resourceType: "SmtpSettings",
            resourceId: command.Id.ToString(),
            metadataJson: null,
            cancellationToken: cancellationToken);

        return settings.ToDto();
    }
}

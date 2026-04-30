using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Audit.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.DTOs;
using Mavrynt.Modules.Notifications.Application.Mapping;
using Mavrynt.Modules.Notifications.Domain.Errors;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed class UpdateSmtpSettingsCommandHandler : ICommandHandler<UpdateSmtpSettingsCommand, SmtpSettingsDto>
{
    private readonly ISmtpSettingsRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter;
    private readonly ISecretProtector _secretProtector;

    public UpdateSmtpSettingsCommandHandler(
        ISmtpSettingsRepository repository,
        IDateTimeProvider dateTimeProvider,
        IAuditLogWriter auditLogWriter,
        ISecretProtector secretProtector)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _auditLogWriter = auditLogWriter;
        _secretProtector = secretProtector;
    }

    public async Task<Result<SmtpSettingsDto>> HandleAsync(
        UpdateSmtpSettingsCommand command,
        CancellationToken cancellationToken = default)
    {
        var idResult = SmtpSettingsId.From(command.Id);
        if (idResult.IsFailure) return idResult.Error;

        var settings = await _repository.GetByIdAsync(idResult.Value, cancellationToken);
        if (settings is null) return NotificationsErrors.SmtpSettingsNotFound;

        string? protectedPassword = command.Password is not null
            ? _secretProtector.Protect(command.Password)
            : null;

        var updateResult = settings.Update(
            command.ProviderName,
            command.Host,
            command.Port,
            command.Username,
            protectedPassword,
            command.SenderEmail,
            command.SenderName,
            command.UseSsl,
            _dateTimeProvider.UtcNow);

        if (updateResult.IsFailure) return updateResult.Error;

        await _auditLogWriter.WriteAsync(
            actorUserId: null,
            action: "SmtpSettingsUpdated",
            resourceType: "SmtpSettings",
            resourceId: command.Id.ToString(),
            metadataJson: null,
            cancellationToken: cancellationToken);

        return settings.ToDto();
    }
}

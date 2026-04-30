using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Audit.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.Abstractions;
using Mavrynt.Modules.Notifications.Application.DTOs;
using Mavrynt.Modules.Notifications.Application.Mapping;
using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Application.Commands;

public sealed class CreateSmtpSettingsCommandHandler : ICommandHandler<CreateSmtpSettingsCommand, SmtpSettingsDto>
{
    private readonly ISmtpSettingsRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter;
    private readonly ISecretProtector _secretProtector;

    public CreateSmtpSettingsCommandHandler(
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
        CreateSmtpSettingsCommand command,
        CancellationToken cancellationToken = default)
    {
        var idResult = SmtpSettingsId.New();
        if (idResult.IsFailure) return idResult.Error;

        var protectedPassword = _secretProtector.Protect(command.Password);

        if (command.IsEnabled)
            await _repository.DisableAllAsync(cancellationToken);

        var settingsResult = SmtpSettings.Create(
            idResult.Value,
            command.ProviderName,
            command.Host,
            command.Port,
            command.Username,
            protectedPassword,
            command.SenderEmail,
            command.SenderName,
            command.UseSsl,
            command.IsEnabled,
            _dateTimeProvider.UtcNow);

        if (settingsResult.IsFailure) return settingsResult.Error;

        await _repository.AddAsync(settingsResult.Value, cancellationToken);

        await _auditLogWriter.WriteAsync(
            actorUserId: null,
            action: "SmtpSettingsCreated",
            resourceType: "SmtpSettings",
            resourceId: idResult.Value.Value.ToString(),
            metadataJson: null,
            cancellationToken: cancellationToken);

        return settingsResult.Value.ToDto();
    }
}

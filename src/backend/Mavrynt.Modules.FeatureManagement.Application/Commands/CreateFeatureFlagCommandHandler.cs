using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Audit.Application.Abstractions;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;
using Mavrynt.Modules.FeatureManagement.Application.Mapping;
using Mavrynt.Modules.FeatureManagement.Domain.Entities;
using Mavrynt.Modules.FeatureManagement.Domain.Errors;
using Mavrynt.Modules.FeatureManagement.Domain.Repositories;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;

namespace Mavrynt.Modules.FeatureManagement.Application.Commands;

public sealed class CreateFeatureFlagCommandHandler : ICommandHandler<CreateFeatureFlagCommand, FeatureFlagDto>
{
    private readonly IFeatureFlagRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter;

    public CreateFeatureFlagCommandHandler(
        IFeatureFlagRepository repository,
        IDateTimeProvider dateTimeProvider,
        IAuditLogWriter auditLogWriter)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<Result<FeatureFlagDto>> HandleAsync(
        CreateFeatureFlagCommand command,
        CancellationToken cancellationToken = default)
    {
        var keyResult = FeatureFlagKey.Create(command.Key);
        if (keyResult.IsFailure)
            return keyResult.Error;

        var keyExists = await _repository.ExistsByKeyAsync(keyResult.Value, cancellationToken);
        if (keyExists)
            return FeatureManagementErrors.KeyAlreadyTaken;

        var idResult = FeatureFlagId.New();
        if (idResult.IsFailure)
            return idResult.Error;

        var flagResult = FeatureFlag.Create(
            idResult.Value,
            keyResult.Value,
            command.Name,
            command.Description,
            command.IsEnabled,
            _dateTimeProvider.UtcNow);

        if (flagResult.IsFailure)
            return flagResult.Error;

        await _repository.AddAsync(flagResult.Value, cancellationToken);

        await _auditLogWriter.WriteAsync(
            actorUserId: null,
            action: "FeatureFlagCreated",
            resourceType: "FeatureFlag",
            resourceId: keyResult.Value.Value,
            metadataJson: null,
            cancellationToken: cancellationToken);

        return flagResult.Value.ToDto();
    }
}

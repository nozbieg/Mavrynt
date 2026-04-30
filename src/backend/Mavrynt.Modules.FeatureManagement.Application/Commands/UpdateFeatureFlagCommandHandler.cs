using Mavrynt.BuildingBlocks.Application.Abstractions;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Audit.Application.Abstractions;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;
using Mavrynt.Modules.FeatureManagement.Application.Mapping;
using Mavrynt.Modules.FeatureManagement.Domain.Errors;
using Mavrynt.Modules.FeatureManagement.Domain.Repositories;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;

namespace Mavrynt.Modules.FeatureManagement.Application.Commands;

public sealed class UpdateFeatureFlagCommandHandler : ICommandHandler<UpdateFeatureFlagCommand, FeatureFlagDto>
{
    private readonly IFeatureFlagRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter;

    public UpdateFeatureFlagCommandHandler(
        IFeatureFlagRepository repository,
        IDateTimeProvider dateTimeProvider,
        IAuditLogWriter auditLogWriter)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<Result<FeatureFlagDto>> HandleAsync(
        UpdateFeatureFlagCommand command,
        CancellationToken cancellationToken = default)
    {
        var keyResult = FeatureFlagKey.Create(command.Key);
        if (keyResult.IsFailure)
            return keyResult.Error;

        var flag = await _repository.GetByKeyAsync(keyResult.Value, cancellationToken);
        if (flag is null)
            return FeatureManagementErrors.FeatureFlagNotFound;

        var updateResult = flag.UpdateDetails(command.Name, command.Description, _dateTimeProvider.UtcNow);
        if (updateResult.IsFailure)
            return updateResult.Error;

        await _auditLogWriter.WriteAsync(
            actorUserId: null,
            action: "FeatureFlagUpdated",
            resourceType: "FeatureFlag",
            resourceId: keyResult.Value.Value,
            metadataJson: null,
            cancellationToken: cancellationToken);

        return flag.ToDto();
    }
}

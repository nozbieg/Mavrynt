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

public sealed class ToggleFeatureFlagCommandHandler : ICommandHandler<ToggleFeatureFlagCommand, FeatureFlagDto>
{
    private readonly IFeatureFlagRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter;

    public ToggleFeatureFlagCommandHandler(
        IFeatureFlagRepository repository,
        IDateTimeProvider dateTimeProvider,
        IAuditLogWriter auditLogWriter)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<Result<FeatureFlagDto>> HandleAsync(
        ToggleFeatureFlagCommand command,
        CancellationToken cancellationToken = default)
    {
        var keyResult = FeatureFlagKey.Create(command.Key);
        if (keyResult.IsFailure)
            return keyResult.Error;

        var flag = await _repository.GetByKeyAsync(keyResult.Value, cancellationToken);
        if (flag is null)
            return FeatureManagementErrors.FeatureFlagNotFound;

        flag.Toggle(_dateTimeProvider.UtcNow);

        await _auditLogWriter.WriteAsync(
            actorUserId: null,
            action: "FeatureFlagToggled",
            resourceType: "FeatureFlag",
            resourceId: keyResult.Value.Value,
            metadataJson: $"{{\"isEnabled\":{flag.IsEnabled.ToString().ToLower()}}}",
            cancellationToken: cancellationToken);

        return flag.ToDto();
    }
}

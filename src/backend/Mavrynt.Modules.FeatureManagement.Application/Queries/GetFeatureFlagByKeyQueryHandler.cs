using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;
using Mavrynt.Modules.FeatureManagement.Application.Mapping;
using Mavrynt.Modules.FeatureManagement.Domain.Errors;
using Mavrynt.Modules.FeatureManagement.Domain.Repositories;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;

namespace Mavrynt.Modules.FeatureManagement.Application.Queries;

public sealed class GetFeatureFlagByKeyQueryHandler : IQueryHandler<GetFeatureFlagByKeyQuery, FeatureFlagDto>
{
    private readonly IFeatureFlagRepository _repository;

    public GetFeatureFlagByKeyQueryHandler(IFeatureFlagRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<FeatureFlagDto>> HandleAsync(
        GetFeatureFlagByKeyQuery query,
        CancellationToken cancellationToken = default)
    {
        var keyResult = FeatureFlagKey.Create(query.Key);
        if (keyResult.IsFailure)
            return keyResult.Error;

        var flag = await _repository.GetByKeyAsync(keyResult.Value, cancellationToken);
        if (flag is null)
            return FeatureManagementErrors.FeatureFlagNotFound;

        return flag.ToDto();
    }
}

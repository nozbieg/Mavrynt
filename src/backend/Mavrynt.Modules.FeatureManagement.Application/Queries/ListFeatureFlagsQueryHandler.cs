using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;
using Mavrynt.Modules.FeatureManagement.Application.Mapping;
using Mavrynt.Modules.FeatureManagement.Domain.Repositories;

namespace Mavrynt.Modules.FeatureManagement.Application.Queries;

public sealed class ListFeatureFlagsQueryHandler : IQueryHandler<ListFeatureFlagsQuery, IReadOnlyList<FeatureFlagDto>>
{
    private readonly IFeatureFlagRepository _repository;

    public ListFeatureFlagsQueryHandler(IFeatureFlagRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<FeatureFlagDto>>> HandleAsync(
        ListFeatureFlagsQuery query,
        CancellationToken cancellationToken = default)
    {
        var flags = await _repository.ListAsync(cancellationToken);
        return Result.Success<IReadOnlyList<FeatureFlagDto>>(flags.Select(f => f.ToDto()).ToList());
    }
}

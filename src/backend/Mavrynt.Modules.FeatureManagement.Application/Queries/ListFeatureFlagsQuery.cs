using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;

namespace Mavrynt.Modules.FeatureManagement.Application.Queries;

public sealed record ListFeatureFlagsQuery : IQuery<IReadOnlyList<FeatureFlagDto>>;

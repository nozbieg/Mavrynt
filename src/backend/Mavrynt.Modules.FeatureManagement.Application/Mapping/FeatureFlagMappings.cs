using Mavrynt.Modules.FeatureManagement.Application.DTOs;
using Mavrynt.Modules.FeatureManagement.Domain.Entities;

namespace Mavrynt.Modules.FeatureManagement.Application.Mapping;

internal static class FeatureFlagMappings
{
    internal static FeatureFlagDto ToDto(this FeatureFlag flag) =>
        new(
            flag.Id.Value,
            flag.Key.Value,
            flag.Name,
            flag.Description,
            flag.IsEnabled,
            flag.CreatedAt,
            flag.UpdatedAt
        );
}

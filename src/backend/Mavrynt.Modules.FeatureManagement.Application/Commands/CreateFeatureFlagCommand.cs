using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;

namespace Mavrynt.Modules.FeatureManagement.Application.Commands;

public sealed record CreateFeatureFlagCommand(
    string Key,
    string Name,
    string? Description,
    bool IsEnabled
) : ICommand<FeatureFlagDto>;

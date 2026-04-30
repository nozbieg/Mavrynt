using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;

namespace Mavrynt.Modules.FeatureManagement.Application.Commands;

public sealed record UpdateFeatureFlagCommand(
    string Key,
    string Name,
    string? Description
) : ICommand<FeatureFlagDto>;

using Mavrynt.BuildingBlocks.Application.Behaviors;
using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.Modules.FeatureManagement.Application.DTOs;

namespace Mavrynt.Modules.FeatureManagement.Application.Commands;

public sealed record ToggleFeatureFlagCommand(string Key) : ICommand<FeatureFlagDto>, ITransactionalRequest;

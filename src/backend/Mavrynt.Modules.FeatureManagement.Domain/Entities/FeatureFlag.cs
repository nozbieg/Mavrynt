using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.FeatureManagement.Domain.Errors;
using Mavrynt.Modules.FeatureManagement.Domain.ValueObjects;

namespace Mavrynt.Modules.FeatureManagement.Domain.Entities;

public sealed class FeatureFlag : AggregateRoot<FeatureFlagId>
{
    private FeatureFlag() : base(null!) { }

    private FeatureFlag(
        FeatureFlagId id,
        FeatureFlagKey key,
        string name,
        string? description,
        bool isEnabled,
        DateTimeOffset createdAt)
        : base(id)
    {
        Key = key;
        Name = name;
        Description = description;
        IsEnabled = isEnabled;
        CreatedAt = createdAt;
    }

    public FeatureFlagKey Key { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsEnabled { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Result<FeatureFlag> Create(
        FeatureFlagId id,
        FeatureFlagKey key,
        string name,
        string? description,
        bool isEnabled,
        DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
            return FeatureManagementErrors.NameEmpty;

        if (name.Trim().Length > 256)
            return FeatureManagementErrors.NameTooLong;

        return new FeatureFlag(id, key, name.Trim(), description?.Trim(), isEnabled, createdAt);
    }

    public Result UpdateDetails(string name, string? description, DateTimeOffset updatedAt)
    {
        if (string.IsNullOrWhiteSpace(name))
            return FeatureManagementErrors.NameEmpty;

        if (name.Trim().Length > 256)
            return FeatureManagementErrors.NameTooLong;

        Name = name.Trim();
        Description = description?.Trim();
        UpdatedAt = updatedAt;

        return Result.Success();
    }

    public Result Enable(DateTimeOffset updatedAt)
    {
        IsEnabled = true;
        UpdatedAt = updatedAt;
        return Result.Success();
    }

    public Result Disable(DateTimeOffset updatedAt)
    {
        IsEnabled = false;
        UpdatedAt = updatedAt;
        return Result.Success();
    }

    public Result Toggle(DateTimeOffset updatedAt)
    {
        IsEnabled = !IsEnabled;
        UpdatedAt = updatedAt;
        return Result.Success();
    }
}

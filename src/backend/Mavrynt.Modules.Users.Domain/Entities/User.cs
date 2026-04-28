using Mavrynt.BuildingBlocks.Domain.Primitives;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Domain.Enums;
using Mavrynt.Modules.Users.Domain.Events;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Domain.Entities;

public sealed class User : AggregateRoot<UserId>
{
    // Private parameterless constructor for ORM compatibility.
    private User() : base(null!)
    {
    }

    private User(
        UserId id,
        Email email,
        PasswordHash passwordHash,
        UserDisplayName? displayName,
        UserStatus status,
        UserRole role,
        DateTimeOffset createdAt)
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        DisplayName = displayName;
        Status = status;
        Role = role;
        CreatedAt = createdAt;
    }

    public Email Email { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public UserDisplayName? DisplayName { get; private set; }
    public UserStatus Status { get; private set; }
    public UserRole Role { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    /// <summary>
    /// Creates and registers a new user with the default <see cref="UserRole.User"/> role.
    /// Raises <see cref="UserRegisteredDomainEvent"/>.
    /// </summary>
    public static User Register(
        UserId id,
        Email email,
        PasswordHash passwordHash,
        UserDisplayName? displayName,
        DateTimeOffset createdAt)
    {
        var user = new User(id, email, passwordHash, displayName, UserStatus.Active, UserRole.User, createdAt);

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(
            Guid.NewGuid(),
            createdAt,
            id,
            email,
            displayName));

        return user;
    }

    /// <summary>
    /// Changes the user's email address.
    /// Raises <see cref="UserEmailChangedDomainEvent"/>.
    /// </summary>
    public Result ChangeEmail(Email newEmail, DateTimeOffset changedAt)
    {
        var oldEmail = Email;
        Email = newEmail;
        UpdatedAt = changedAt;

        RaiseDomainEvent(new UserEmailChangedDomainEvent(
            Guid.NewGuid(),
            changedAt,
            Id,
            oldEmail,
            newEmail));

        return Result.Success();
    }

    /// <summary>
    /// Replaces the stored password hash.
    /// Raises <see cref="UserPasswordChangedDomainEvent"/>.
    /// Does not perform hashing.
    /// </summary>
    public Result ChangePasswordHash(PasswordHash newPasswordHash, DateTimeOffset changedAt)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = changedAt;

        RaiseDomainEvent(new UserPasswordChangedDomainEvent(
            Guid.NewGuid(),
            changedAt,
            Id));

        return Result.Success();
    }

    /// <summary>
    /// Sets or replaces the user's display name.
    /// </summary>
    public Result ChangeDisplayName(UserDisplayName? newDisplayName, DateTimeOffset changedAt)
    {
        DisplayName = newDisplayName;
        UpdatedAt = changedAt;
        return Result.Success();
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    public Result Activate(DateTimeOffset changedAt)
    {
        Status = UserStatus.Active;
        UpdatedAt = changedAt;
        return Result.Success();
    }

    /// <summary>
    /// Deactivates the user account.
    /// </summary>
    public Result Deactivate(DateTimeOffset changedAt)
    {
        Status = UserStatus.Inactive;
        UpdatedAt = changedAt;
        return Result.Success();
    }

    /// <summary>
    /// Assigns a new role to the user.
    /// Extension point for future role-management flows.
    /// </summary>
    public Result AssignRole(UserRole newRole, DateTimeOffset changedAt)
    {
        Role = newRole;
        UpdatedAt = changedAt;
        return Result.Success();
    }
}

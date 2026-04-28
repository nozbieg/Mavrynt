namespace Mavrynt.Modules.Users.Domain.Enums;

/// <summary>
/// The role assigned to a user. Used for coarse-grained authorization (e.g. AdminOnly policy).
/// Full RBAC can be layered on top of this in a future phase.
/// </summary>
public enum UserRole
{
    User = 0,
    Admin = 1
}

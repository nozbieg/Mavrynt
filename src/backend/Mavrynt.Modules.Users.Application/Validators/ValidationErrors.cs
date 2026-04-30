using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.Modules.Users.Application.Validators;

/// <summary>
/// Input validation errors for the Users application layer.
/// These are surface-level errors produced before the domain is invoked.
/// </summary>
internal static class ValidationErrors
{
    public static readonly Error EmailRequired =
        new("Validation.EmailRequired", "Email address is required.");

    public static readonly Error EmailTooLong =
        new("Validation.EmailTooLong", "Email address must not exceed 320 characters.");

    public static readonly Error PasswordRequired =
        new("Validation.PasswordRequired", "Password is required.");

    public static readonly Error PasswordTooShort =
        new("Validation.PasswordTooShort", "Password must be at least 8 characters.");

    public static readonly Error UserIdRequired =
        new("Validation.UserIdRequired", "A valid UserId is required.");

    public static readonly Error RoleRequired =
        new("Validation.RoleRequired", "Role is required.");
}

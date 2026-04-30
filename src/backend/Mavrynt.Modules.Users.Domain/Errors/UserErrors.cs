using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.Modules.Users.Domain.Errors;

public static class UserErrors
{
    // UserId
    public static readonly Error InvalidUserId =
        new("Users.UserId.Invalid", "User ID must not be an empty GUID.");

    // Email
    public static readonly Error EmailEmpty =
        new("Users.Email.Empty", "Email address must not be empty.");

    public static readonly Error EmailTooLong =
        new("Users.Email.TooLong", $"Email address must not exceed {ValueObjects.Email.MaxLength} characters.");

    public static readonly Error EmailInvalid =
        new("Users.Email.Invalid", "Email address is not in a valid format.");

    // PasswordHash
    public static readonly Error PasswordHashEmpty =
        new("Users.PasswordHash.Empty", "Password hash must not be empty.");

    public static readonly Error PasswordHashTooLong =
        new("Users.PasswordHash.TooLong", $"Password hash must not exceed {ValueObjects.PasswordHash.MaxLength} characters.");

    // DisplayName
    public static readonly Error DisplayNameEmpty =
        new("Users.DisplayName.Empty", "Display name must not be empty when provided.");

    public static readonly Error DisplayNameTooLong =
        new("Users.DisplayName.TooLong", $"Display name must not exceed {ValueObjects.UserDisplayName.MaxLength} characters.");

    // User aggregate
    public static readonly Error UserNotFound =
        new("Users.User.NotFound", "The requested user was not found.");

    public static readonly Error EmailAlreadyTaken =
        new("Users.User.EmailAlreadyTaken", "The provided email address is already in use.");

    public static readonly Error InvalidCredentials =
        new("Users.User.InvalidCredentials", "The provided credentials are invalid.");

    public static readonly Error InvalidRole =
        new("Users.User.InvalidRole", "The provided role value is not valid. Accepted values: User, Admin.");

    public static readonly Error InvalidCurrentPassword =
        new("Users.User.InvalidCurrentPassword", "The provided current password is incorrect.");
}

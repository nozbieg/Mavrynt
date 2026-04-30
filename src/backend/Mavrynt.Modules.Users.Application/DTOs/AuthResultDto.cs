namespace Mavrynt.Modules.Users.Application.DTOs;

/// <summary>
/// Returned by the login endpoint and consumed by <c>@mavrynt/auth-ui</c>.
/// </summary>
public sealed record AuthResultDto(
    UserDto User,
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAt,
    bool RequiresPasswordChange
);

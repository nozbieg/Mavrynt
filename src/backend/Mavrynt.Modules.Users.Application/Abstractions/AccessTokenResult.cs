namespace Mavrynt.Modules.Users.Application.Abstractions;

/// <summary>
/// The result of a successful JWT access-token generation.
/// </summary>
public sealed record AccessTokenResult(
    string Token,
    DateTimeOffset ExpiresAt
);

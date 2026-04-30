namespace Mavrynt.Modules.Users.Application.Abstractions;

/// <summary>
/// Generates JWT access tokens for authenticated users.
/// The implementation lives in Infrastructure; this abstraction keeps Application
/// free of token-signing and cryptographic details.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Creates a signed JWT access token carrying the provided user claims.
    /// </summary>
    AccessTokenResult GenerateToken(
        Guid userId,
        string email,
        string? displayName,
        string role,
        bool requiresPasswordChange);
}

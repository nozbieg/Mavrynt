namespace Mavrynt.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Abstraction for password hashing and verification.
/// Implementations must live in Infrastructure; raw passwords must never be stored or logged.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Produces a one-way hash of <paramref name="password"/>.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies that <paramref name="password"/> matches <paramref name="passwordHash"/>.
    /// Returns <c>false</c> on any mismatch — never throws on bad input.
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}

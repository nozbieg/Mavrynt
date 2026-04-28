namespace Mavrynt.Modules.Users.Infrastructure.Security;

/// <summary>
/// Strongly-typed JWT configuration.
/// Bound from the <c>Jwt</c> section in appsettings.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>
    /// Secret key used to sign tokens. Must be at least 32 characters.
    /// In production, inject via environment variable or secrets manager — never commit.
    /// </summary>
    public string Secret { get; init; } = string.Empty;

    /// <summary>The <c>iss</c> claim value. Should match the validating host(s).</summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>The <c>aud</c> claim value. Should match the validating host(s).</summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>Token lifetime in minutes. Default: 60.</summary>
    public int ExpirationMinutes { get; init; } = 60;
}

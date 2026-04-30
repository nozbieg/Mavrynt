using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mavrynt.Modules.Users.Application.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Mavrynt.Modules.Users.Infrastructure.Security;

/// <summary>
/// Issues signed JWT Bearer access tokens using HMAC-SHA-256.
/// Token value must never be logged.
/// </summary>
internal sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AccessTokenResult GenerateToken(
        Guid userId,
        string email,
        string? displayName,
        string role,
        bool requiresPasswordChange)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTimeOffset.UtcNow;
        var expires = now.AddMinutes(_options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // ClaimTypes.Role is recognised by ASP.NET Core's policy engine
            // and maps correctly to RequireRole / [Authorize(Roles = "...")].
            new(ClaimTypes.Role, role),
            new("requires_password_change", requiresPasswordChange ? "true" : "false"),
        };

        if (displayName is not null)
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, displayName));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult(tokenString, expires);
    }
}

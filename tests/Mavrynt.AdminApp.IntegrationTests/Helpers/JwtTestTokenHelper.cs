using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Mavrynt.AdminApp.IntegrationTests.Helpers;

internal static class JwtTestTokenHelper
{
    private const string Issuer = "mavrynt-tests";
    private const string Audience = "mavrynt-tests";
    public const string TestSecret = "0123456789abcdef0123456789abcdef";
    private const string Secret = TestSecret;

    public static string GenerateAdminToken(Guid? userId = null)
        => GenerateToken(userId ?? Guid.NewGuid(), "admin@test.com", "Admin");

    public static string GenerateUserToken(Guid? userId = null)
        => GenerateToken(userId ?? Guid.NewGuid(), "user@test.com", "User");

    private static string GenerateToken(Guid userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", role),
        };

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

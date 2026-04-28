using System.Security.Claims;
using System.Text;
using Mavrynt.Modules.Users.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Mavrynt.Api.DependencyInjection;

public static class AuthServiceCollectionExtensions
{
    /// <summary>
    /// Registers JWT Bearer authentication and the shared authorization policies.
    /// Both <c>Mavrynt.Api</c> and <c>Mavrynt.AdminApp</c> use the same configuration shape.
    /// </summary>
    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                $"JWT configuration section '{JwtOptions.SectionName}' is missing or empty.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Keep original claim names from the token — no automatic remapping.
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Secret)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),

                    // "sub" claim maps to Identity.Name; ClaimTypes.Role maps to role checks.
                    NameClaimType = "sub",
                    RoleClaimType = ClaimTypes.Role,
                };
            });

        services.AddAuthorization(options =>
        {
            // Used by Mavrynt.AdminApp to protect admin-only endpoints.
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
            });
        });

        return services;
    }
}

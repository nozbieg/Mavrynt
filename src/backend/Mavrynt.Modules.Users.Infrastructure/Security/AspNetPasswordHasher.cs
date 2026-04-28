using Mavrynt.BuildingBlocks.Application.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Mavrynt.Modules.Users.Infrastructure.Security;

/// <summary>
/// PBKDF2-based password hasher backed by ASP.NET Core Identity's <see cref="PasswordHasher{TUser}"/>.
/// Uses the v3 format (PBKDF2 with HMAC-SHA512, 100 000 iterations) — appropriate for production use.
/// </summary>
internal sealed class AspNetPasswordHasher : IPasswordHasher
{
    // TUser is unused by the default implementation; object is a safe placeholder.
    private static readonly PasswordHasher<object> _hasher = new();

    public string HashPassword(string password)
        => _hasher.HashPassword(null!, password);

    public bool VerifyPassword(string password, string passwordHash)
    {
        var result = _hasher.VerifyHashedPassword(null!, passwordHash, password);
        // SuccessRehashNeeded means the hash is valid but was created with an older algorithm.
        // Treat it as success — a future password change will upgrade the hash automatically.
        return result != PasswordVerificationResult.Failed;
    }
}

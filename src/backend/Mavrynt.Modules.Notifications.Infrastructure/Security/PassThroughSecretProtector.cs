using Mavrynt.Modules.Notifications.Application.Abstractions;

namespace Mavrynt.Modules.Notifications.Infrastructure.Security;

/// <summary>
/// Development-only pass-through secret protector. Stores values in plaintext.
/// Replace with a real implementation (DPAPI, Azure Key Vault, etc.) before production.
/// </summary>
internal sealed class PassThroughSecretProtector : ISecretProtector
{
    public string Protect(string plaintext) => plaintext;
    public string Unprotect(string protectedValue) => protectedValue;
}

namespace Mavrynt.Modules.Notifications.Application.Abstractions;

/// <summary>
/// Abstraction for protecting and unprotecting secret values (e.g. SMTP passwords).
/// The default development implementation is pass-through. Replace with real encryption
/// (KeyVault, DPAPI, etc.) before going to production.
/// </summary>
public interface ISecretProtector
{
    string Protect(string plaintext);
    string Unprotect(string protectedValue);
}

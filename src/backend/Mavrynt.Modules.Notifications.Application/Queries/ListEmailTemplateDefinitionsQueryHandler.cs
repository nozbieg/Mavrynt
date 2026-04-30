using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.DTOs;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;

namespace Mavrynt.Modules.Notifications.Application.Queries;

public sealed class ListEmailTemplateDefinitionsQueryHandler
    : IQueryHandler<ListEmailTemplateDefinitionsQuery, IReadOnlyList<EmailTemplateDefinitionDto>>
{
    private static readonly IReadOnlyList<EmailTemplateDefinitionDto> Definitions =
    [
        new(
            TemplateKey: EmailTemplateKey.LoginConfirmation,
            DisplayName: "Login Confirmation",
            Description: "Sent when a user logs in. Notifies them of the login time, IP, and user agent.",
            SupportedPlaceholders: ["UserEmail", "DisplayName", "LoginAt", "IpAddress", "UserAgent"]),
        new(
            TemplateKey: EmailTemplateKey.PasswordReset,
            DisplayName: "Password Reset",
            Description: "Sent when a user requests a password reset. Contains the reset link and its expiry.",
            SupportedPlaceholders: ["UserEmail", "DisplayName", "ResetLink", "ExpiresAt"]),
        new(
            TemplateKey: EmailTemplateKey.TwoFactorCode,
            DisplayName: "Two-Factor Authentication Code",
            Description: "Sent when a user requests a 2FA verification code.",
            SupportedPlaceholders: ["UserEmail", "DisplayName", "Code", "ExpiresAt"]),
    ];

    public Task<Result<IReadOnlyList<EmailTemplateDefinitionDto>>> HandleAsync(
        ListEmailTemplateDefinitionsQuery query,
        CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(Definitions));
}

using Mavrynt.Modules.Notifications.Domain.Entities;
using Mavrynt.Modules.Notifications.Domain.Repositories;
using Mavrynt.Modules.Notifications.Domain.ValueObjects;
using Mavrynt.Modules.Notifications.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Mavrynt.Modules.Notifications.Infrastructure.Seeding;

internal sealed class DefaultEmailTemplateSeeder
{
    private readonly IEmailTemplateRepository _repository;
    private readonly NotificationsDbContext _context;
    private readonly ILogger<DefaultEmailTemplateSeeder> _logger;

    public DefaultEmailTemplateSeeder(
        IEmailTemplateRepository repository,
        NotificationsDbContext context,
        ILogger<DefaultEmailTemplateSeeder> logger)
    {
        _repository = repository;
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var templates = BuildDefaultTemplates();

        foreach (var (key, template) in templates)
        {
            var exists = await _repository.ExistsByKeyAsync(key, cancellationToken);
            if (exists)
            {
                _logger.LogDebug("Email template {TemplateKey} already exists, skipping seed.", key.Value);
                continue;
            }

            await _repository.AddAsync(template, cancellationToken);
            _logger.LogInformation("Seeded default email template: {TemplateKey}", key.Value);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static IReadOnlyList<(EmailTemplateKey Key, EmailTemplate Template)> BuildDefaultTemplates()
    {
        var now = DateTimeOffset.UtcNow;

        var loginKey = EmailTemplateKey.Create(EmailTemplateKey.LoginConfirmation).Value;
        var loginIdResult = EmailTemplateId.New();
        var loginTemplate = EmailTemplate.Create(
            loginIdResult.Value,
            loginKey,
            "Login Confirmation",
            "Notifies a user of a new login to their Mavrynt account.",
            "New login to your Mavrynt account",
            BuildLoginConfirmationHtml(),
            BuildLoginConfirmationText(),
            isEnabled: true,
            createdAt: now).Value;

        var passwordKey = EmailTemplateKey.Create(EmailTemplateKey.PasswordReset).Value;
        var passwordIdResult = EmailTemplateId.New();
        var passwordTemplate = EmailTemplate.Create(
            passwordIdResult.Value,
            passwordKey,
            "Password Reset",
            "Delivers a password reset link to the user.",
            "Reset your Mavrynt password",
            BuildPasswordResetHtml(),
            BuildPasswordResetText(),
            isEnabled: true,
            createdAt: now).Value;

        var twoFactorKey = EmailTemplateKey.Create(EmailTemplateKey.TwoFactorCode).Value;
        var twoFactorIdResult = EmailTemplateId.New();
        var twoFactorTemplate = EmailTemplate.Create(
            twoFactorIdResult.Value,
            twoFactorKey,
            "Two-Factor Authentication Code",
            "Delivers a two-factor authentication code to the user.",
            "Your Mavrynt verification code",
            BuildTwoFactorHtml(),
            BuildTwoFactorText(),
            isEnabled: true,
            createdAt: now).Value;

        return
        [
            (loginKey, loginTemplate),
            (passwordKey, passwordTemplate),
            (twoFactorKey, twoFactorTemplate),
        ];
    }

    private static string BuildLoginConfirmationHtml() => """
        <!DOCTYPE html>
        <html>
        <body style="font-family: sans-serif; color: #333;">
          <h2>New login to your Mavrynt account</h2>
          <p>Hello {{DisplayName}},</p>
          <p>We detected a new login to your account (<strong>{{UserEmail}}</strong>).</p>
          <ul>
            <li><strong>Time:</strong> {{LoginAt}}</li>
            <li><strong>IP Address:</strong> {{IpAddress}}</li>
            <li><strong>User Agent:</strong> {{UserAgent}}</li>
          </ul>
          <p>If this was you, no action is needed. If you do not recognise this login, please change your password immediately.</p>
          <p>The Mavrynt Team</p>
        </body>
        </html>
        """;

    private static string BuildLoginConfirmationText() =>
        "New login to your Mavrynt account\n\n" +
        "Hello {{DisplayName}},\n\n" +
        "A new login was detected for {{UserEmail}} at {{LoginAt}} from {{IpAddress}} ({{UserAgent}}).\n\n" +
        "If this was not you, change your password immediately.\n\n" +
        "The Mavrynt Team";

    private static string BuildPasswordResetHtml() => """
        <!DOCTYPE html>
        <html>
        <body style="font-family: sans-serif; color: #333;">
          <h2>Reset your Mavrynt password</h2>
          <p>Hello {{DisplayName}},</p>
          <p>We received a request to reset the password for <strong>{{UserEmail}}</strong>.</p>
          <p>
            <a href="{{ResetLink}}" style="background:#4f46e5;color:#fff;padding:10px 20px;border-radius:4px;text-decoration:none;">
              Reset Password
            </a>
          </p>
          <p>This link expires at <strong>{{ExpiresAt}}</strong>.</p>
          <p>If you did not request a password reset, you can ignore this email.</p>
          <p>The Mavrynt Team</p>
        </body>
        </html>
        """;

    private static string BuildPasswordResetText() =>
        "Reset your Mavrynt password\n\n" +
        "Hello {{DisplayName}},\n\n" +
        "Reset link for {{UserEmail}}: {{ResetLink}}\n" +
        "This link expires at {{ExpiresAt}}.\n\n" +
        "If you did not request this, ignore this email.\n\n" +
        "The Mavrynt Team";

    private static string BuildTwoFactorHtml() => """
        <!DOCTYPE html>
        <html>
        <body style="font-family: sans-serif; color: #333;">
          <h2>Your Mavrynt verification code</h2>
          <p>Hello {{DisplayName}},</p>
          <p>Your verification code for <strong>{{UserEmail}}</strong> is:</p>
          <p style="font-size:2em;letter-spacing:0.2em;font-weight:bold;color:#4f46e5;">{{Code}}</p>
          <p>This code expires at <strong>{{ExpiresAt}}</strong>.</p>
          <p>Do not share this code with anyone.</p>
          <p>The Mavrynt Team</p>
        </body>
        </html>
        """;

    private static string BuildTwoFactorText() =>
        "Your Mavrynt verification code\n\n" +
        "Hello {{DisplayName}},\n\n" +
        "Your verification code for {{UserEmail}} is: {{Code}}\n" +
        "This code expires at {{ExpiresAt}}.\n\n" +
        "Do not share this code with anyone.\n\n" +
        "The Mavrynt Team";
}

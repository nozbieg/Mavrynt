using Mavrynt.BuildingBlocks.Application.Validation;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.Commands;
using Mavrynt.Modules.Notifications.Domain.Errors;

namespace Mavrynt.Modules.Notifications.Application.Validators;

public sealed class SendSmtpTestEmailCommandValidator : IValidator<SendSmtpTestEmailCommand>
{
    public Task<Result> ValidateAsync(
        SendSmtpTestEmailCommand request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RecipientEmail))
            return Task.FromResult(Result.Failure(NotificationsErrors.EmailRecipientInvalid));

        var trimmed = request.RecipientEmail.Trim();
        var atIndex = trimmed.IndexOf('@');
        if (atIndex <= 0 || atIndex >= trimmed.Length - 1)
            return Task.FromResult(Result.Failure(NotificationsErrors.EmailRecipientInvalid));

        var domain = trimmed[(atIndex + 1)..];
        if (!domain.Contains('.'))
            return Task.FromResult(Result.Failure(NotificationsErrors.EmailRecipientInvalid));

        return Task.FromResult(Result.Success());
    }
}

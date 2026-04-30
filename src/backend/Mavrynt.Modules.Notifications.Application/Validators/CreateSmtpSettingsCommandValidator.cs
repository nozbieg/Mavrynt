using Mavrynt.BuildingBlocks.Application.Validation;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.Commands;
using Mavrynt.Modules.Notifications.Domain.Errors;

namespace Mavrynt.Modules.Notifications.Application.Validators;

public sealed class CreateSmtpSettingsCommandValidator : IValidator<CreateSmtpSettingsCommand>
{
    public Task<Result> ValidateAsync(CreateSmtpSettingsCommand request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ProviderName))
            return Task.FromResult(Result.Failure(NotificationsErrors.SmtpSettingsProviderNameEmpty));
        if (string.IsNullOrWhiteSpace(request.Host))
            return Task.FromResult(Result.Failure(NotificationsErrors.SmtpSettingsHostEmpty));
        if (request.Port is < 1 or > 65535)
            return Task.FromResult(Result.Failure(NotificationsErrors.SmtpSettingsPortInvalid));
        if (string.IsNullOrWhiteSpace(request.Username))
            return Task.FromResult(Result.Failure(NotificationsErrors.SmtpSettingsUsernameEmpty));
        if (string.IsNullOrWhiteSpace(request.Password))
            return Task.FromResult(Result.Failure(NotificationsErrors.SmtpSettingsPasswordEmpty));
        if (string.IsNullOrWhiteSpace(request.SenderEmail))
            return Task.FromResult(Result.Failure(NotificationsErrors.SmtpSettingsSenderEmailEmpty));

        return Task.FromResult(Result.Success());
    }
}

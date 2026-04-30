using Mavrynt.BuildingBlocks.Application.Validation;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Notifications.Application.Commands;
using Mavrynt.Modules.Notifications.Domain.Errors;

namespace Mavrynt.Modules.Notifications.Application.Validators;

public sealed class SendTestEmailCommandValidator : IValidator<SendTestEmailCommand>
{
    public Task<Result> ValidateAsync(SendTestEmailCommand request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RecipientEmail))
            return Task.FromResult(Result.Failure(
                new Error("Notifications.TestEmail.RecipientRequired", "Recipient email must not be empty.")));

        if (!request.RecipientEmail.Contains('@'))
            return Task.FromResult(Result.Failure(
                new Error("Notifications.TestEmail.RecipientInvalid", "Recipient email is not a valid address.")));

        return Task.FromResult(Result.Success());
    }
}

using Mavrynt.BuildingBlocks.Application.Validation;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.Commands;

namespace Mavrynt.Modules.Users.Application.Validators;

/// <summary>
/// Input validation for <see cref="ChangeOwnPasswordCommand"/>.
/// </summary>
public sealed class ChangeOwnPasswordCommandValidator : IValidator<ChangeOwnPasswordCommand>
{
    private const int MinPasswordLength = 8;

    public Task<Result> ValidateAsync(ChangeOwnPasswordCommand request, CancellationToken cancellationToken = default)
    {
        if (request.UserId == Guid.Empty)
            return Task.FromResult(Result.Failure(ValidationErrors.UserIdRequired));

        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            return Task.FromResult(Result.Failure(ValidationErrors.PasswordRequired));

        if (string.IsNullOrWhiteSpace(request.NewPassword))
            return Task.FromResult(Result.Failure(ValidationErrors.PasswordRequired));

        if (request.NewPassword.Length < MinPasswordLength)
            return Task.FromResult(Result.Failure(ValidationErrors.PasswordTooShort));

        return Task.FromResult(Result.Success());
    }
}

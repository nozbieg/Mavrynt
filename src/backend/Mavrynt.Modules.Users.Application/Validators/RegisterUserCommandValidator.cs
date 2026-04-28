using Mavrynt.BuildingBlocks.Application.Validation;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.Commands;

namespace Mavrynt.Modules.Users.Application.Validators;

/// <summary>
/// Input validation for <see cref="RegisterUserCommand"/>.
/// Checks surface-level constraints before the handler invokes domain logic.
///
/// Domain invariants (email format, password strength) are enforced
/// by value objects in the Domain layer — do not duplicate them here.
/// </summary>
public sealed class RegisterUserCommandValidator : IValidator<RegisterUserCommand>
{
    private const int MinPasswordLength = 8;
    private const int MaxEmailLength = 320;

    public Task<Result> ValidateAsync(RegisterUserCommand request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return Task.FromResult(Result.Failure(ValidationErrors.EmailRequired));

        if (request.Email.Length > MaxEmailLength)
            return Task.FromResult(Result.Failure(ValidationErrors.EmailTooLong));

        if (string.IsNullOrWhiteSpace(request.Password))
            return Task.FromResult(Result.Failure(ValidationErrors.PasswordRequired));

        if (request.Password.Length < MinPasswordLength)
            return Task.FromResult(Result.Failure(ValidationErrors.PasswordTooShort));

        return Task.FromResult(Result.Success());
    }
}

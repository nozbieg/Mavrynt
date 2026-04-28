using Mavrynt.BuildingBlocks.Application.Validation;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.Commands;

namespace Mavrynt.Modules.Users.Application.Validators;

/// <summary>
/// Input validation for <see cref="LoginUserCommand"/>.
/// Only checks that required fields are present — credential correctness
/// is verified by the handler (constant-time comparison to prevent timing attacks).
/// Never return a specific error that reveals which field was wrong.
/// </summary>
public sealed class LoginUserCommandValidator : IValidator<LoginUserCommand>
{
    public Task<Result> ValidateAsync(LoginUserCommand request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return Task.FromResult(Result.Failure(ValidationErrors.EmailRequired));

        if (string.IsNullOrWhiteSpace(request.Password))
            return Task.FromResult(Result.Failure(ValidationErrors.PasswordRequired));

        return Task.FromResult(Result.Success());
    }
}

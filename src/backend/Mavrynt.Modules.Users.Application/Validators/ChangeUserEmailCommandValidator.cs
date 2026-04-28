using Mavrynt.BuildingBlocks.Application.Validation;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.Commands;

namespace Mavrynt.Modules.Users.Application.Validators;

/// <summary>
/// Input validation for <see cref="ChangeUserEmailCommand"/>.
/// </summary>
public sealed class ChangeUserEmailCommandValidator : IValidator<ChangeUserEmailCommand>
{
    private const int MaxEmailLength = 320;

    public Task<Result> ValidateAsync(ChangeUserEmailCommand request, CancellationToken cancellationToken = default)
    {
        if (request.UserId == Guid.Empty)
            return Task.FromResult(Result.Failure(ValidationErrors.UserIdRequired));

        if (string.IsNullOrWhiteSpace(request.NewEmail))
            return Task.FromResult(Result.Failure(ValidationErrors.EmailRequired));

        if (request.NewEmail.Length > MaxEmailLength)
            return Task.FromResult(Result.Failure(ValidationErrors.EmailTooLong));

        return Task.FromResult(Result.Success());
    }
}

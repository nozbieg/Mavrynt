using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.BuildingBlocks.Application.Validation;

/// <summary>
/// Validates a request before it reaches the handler.
/// Multiple validators per request type are supported — they run sequentially
/// and stop on the first failure.
///
/// Return <see cref="Result.Success()"/> when valid.
/// Return <see cref="Result.Failure(Error)"/> with a descriptive error when invalid.
///
/// Validators are discovered automatically by <c>AddMavryntMediator</c>
/// via assembly scanning. Do not add FluentValidation or other libraries
/// unless explicitly decided and documented in an ADR.
/// </summary>
public interface IValidator<in TRequest>
{
    Task<Result> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
}

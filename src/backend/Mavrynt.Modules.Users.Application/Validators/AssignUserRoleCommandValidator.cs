using Mavrynt.BuildingBlocks.Application.Validation;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.Commands;

namespace Mavrynt.Modules.Users.Application.Validators;

public sealed class AssignUserRoleCommandValidator : IValidator<AssignUserRoleCommand>
{
    public Task<Result> ValidateAsync(AssignUserRoleCommand request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Role))
            return Task.FromResult(Result.Failure(ValidationErrors.RoleRequired));

        return Task.FromResult(Result.Success());
    }
}

using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Mapping;
using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Application.Commands;

public sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;

    public LoginUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<AuthResultDto>> HandleAsync(
        LoginUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var suppliedHashResult = PasswordHash.Create(command.PasswordHash);
        if (suppliedHashResult.IsFailure)
            return suppliedHashResult.Error;

        var user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null)
            return UserErrors.InvalidCredentials;

        var hashesMatch = string.Equals(
            user.PasswordHash.Value,
            suppliedHashResult.Value.Value,
            StringComparison.Ordinal);

        if (!hashesMatch)
            return UserErrors.InvalidCredentials;

        // Token strategy is intentionally deferred — TokenType is a reserved placeholder.
        var result = new AuthResultDto(user.ToDto(), TokenType: "not_implemented");
        return result;
    }
}

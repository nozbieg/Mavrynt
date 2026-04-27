using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Mapping;
using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Application.Queries;

public sealed class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserDto>> HandleAsync(
        GetUserByEmailQuery query,
        CancellationToken cancellationToken = default)
    {
        var emailResult = Email.Create(query.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null)
            return UserErrors.UserNotFound;

        return user.ToDto();
    }
}

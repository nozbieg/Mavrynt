using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Mapping;
using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.Repositories;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Application.Queries;

public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserDto>> HandleAsync(
        GetUserByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = UserId.From(query.UserId);
        if (userIdResult.IsFailure)
            return userIdResult.Error;

        var user = await _userRepository.GetByIdAsync(userIdResult.Value, cancellationToken);
        if (user is null)
            return UserErrors.UserNotFound;

        return user.ToDto();
    }
}

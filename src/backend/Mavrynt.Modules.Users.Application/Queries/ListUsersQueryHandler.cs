using Mavrynt.BuildingBlocks.Application.Messaging;
using Mavrynt.BuildingBlocks.Domain.Results;
using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Application.Mapping;
using Mavrynt.Modules.Users.Domain.Repositories;

namespace Mavrynt.Modules.Users.Application.Queries;

public sealed class ListUsersQueryHandler : IQueryHandler<ListUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public ListUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<IReadOnlyList<UserDto>>> HandleAsync(
        ListUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        IReadOnlyList<UserDto> dtos = users.Select(u => u.ToDto()).ToList();
        return Result.Success(dtos);
    }
}

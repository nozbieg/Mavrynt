using Mavrynt.Modules.Users.Application.DTOs;
using Mavrynt.Modules.Users.Domain.Entities;

namespace Mavrynt.Modules.Users.Application.Mapping;

internal static class UserMappings
{
    internal static UserDto ToDto(this User user) =>
        new(
            user.Id.Value,
            user.Email.Value,
            user.DisplayName?.Value,
            user.Status.ToString(),
            user.Role.ToString(),
            user.CreatedAt,
            user.UpdatedAt,
            user.RequiresPasswordChange
        );
}

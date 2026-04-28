namespace Mavrynt.Modules.Users.Application.DTOs;

public sealed record UserDto(
    Guid Id,
    string Email,
    string? DisplayName,
    string Status,
    string Role,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);

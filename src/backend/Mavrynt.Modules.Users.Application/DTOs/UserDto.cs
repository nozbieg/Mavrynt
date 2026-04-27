namespace Mavrynt.Modules.Users.Application.DTOs;

public sealed record UserDto(
    Guid Id,
    string Email,
    string? DisplayName,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);

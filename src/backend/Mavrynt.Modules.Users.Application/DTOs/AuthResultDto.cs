namespace Mavrynt.Modules.Users.Application.DTOs;

public sealed record AuthResultDto(
    UserDto User,
    string TokenType
);

using Mavrynt.Modules.Users.Application.Queries;
using Mavrynt.Modules.Users.Application.Tests.Fakes;
using Mavrynt.Modules.Users.Domain.Entities;
using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.ValueObjects;
using Xunit;

namespace Mavrynt.Modules.Users.Application.Tests;

public sealed class QueryHandlersTests
{
    [Fact]
    public async Task GetById_Should_Return_Dto_When_Found()
    {
        var repository = new InMemoryUserRepository();
        var user = CreateUser("john@example.com");
        repository.Seed(user);

        var result = await new GetUserByIdQueryHandler(repository).HandleAsync(new GetUserByIdQuery(user.Id.Value));

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id.Value, result.Value.Id);
        Assert.Equal("UserDto", result.Value.GetType().Name);
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Missing()
    {
        var result = await new GetUserByIdQueryHandler(new InMemoryUserRepository()).HandleAsync(new GetUserByIdQuery(Guid.NewGuid()));

        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task GetByEmail_Should_Return_Dto_When_Found()
    {
        var repository = new InMemoryUserRepository();
        repository.Seed(CreateUser("john@example.com"));

        var result = await new GetUserByEmailQueryHandler(repository).HandleAsync(new GetUserByEmailQuery("john@example.com"));

        Assert.True(result.IsSuccess);
        Assert.Equal("john@example.com", result.Value.Email);
    }

    [Fact]
    public async Task GetByEmail_Should_Return_NotFound_When_Missing()
    {
        var result = await new GetUserByEmailQueryHandler(new InMemoryUserRepository()).HandleAsync(new GetUserByEmailQuery("missing@example.com"));

        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.UserNotFound, result.Error);
    }

    private static User CreateUser(string email) =>
        User.Register(UserId.New().Value, Email.Create(email).Value, PasswordHash.Create("hashed::Secret1").Value, UserDisplayName.Create("John").Value, DateTimeOffset.UtcNow);
}

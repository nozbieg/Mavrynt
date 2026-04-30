using Mavrynt.Modules.Users.Application.Commands;
using Mavrynt.Modules.Users.Application.Tests.Fakes;
using Mavrynt.Modules.Users.Domain.Entities;
using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.ValueObjects;
using Xunit;

namespace Mavrynt.Modules.Users.Application.Tests;

public sealed class CommandHandlersTests
{
    [Fact]
    public async Task Register_Should_Return_Success_And_Persist_User()
    {
        var repository = new InMemoryUserRepository();
        var audit = new FakeAuditService();
        var handler = new RegisterUserCommandHandler(repository, new FixedDateTimeProvider(DateTimeOffset.UtcNow), new FakePasswordHasher(), audit);

        var result = await handler.HandleAsync(new RegisterUserCommand("john@example.com", "Secret1", "John"));

        Assert.True(result.IsSuccess);
        Assert.Single(repository.Users);
        Assert.Single(audit.Entries, x => x.EventType == "user_registered");
    }

    [Fact]
    public async Task Register_Should_Reject_Duplicated_Email()
    {
        var repository = new InMemoryUserRepository();
        repository.Seed(CreateUser("john@example.com"));
        var handler = new RegisterUserCommandHandler(repository, new FixedDateTimeProvider(DateTimeOffset.UtcNow), new FakePasswordHasher(), new FakeAuditService());

        var result = await handler.HandleAsync(new RegisterUserCommand("john@example.com", "Secret1", "John"));

        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.EmailAlreadyTaken, result.Error);
    }

    [Fact]
    public async Task Register_Should_Return_Error_For_Invalid_Email()
    {
        var handler = new RegisterUserCommandHandler(new InMemoryUserRepository(), new FixedDateTimeProvider(DateTimeOffset.UtcNow), new FakePasswordHasher(), new FakeAuditService());
        var result = await handler.HandleAsync(new RegisterUserCommand("invalid", "Secret1", "John"));

        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.EmailInvalid, result.Error);
    }

    [Fact]
    public async Task Login_Should_Fail_When_User_Does_Not_Exist()
    {
        var handler = new LoginUserCommandHandler(new InMemoryUserRepository(), new FakePasswordHasher(), new FakeJwtTokenService(), new FixedDateTimeProvider(DateTimeOffset.UtcNow), new FakeAuditService());
        var result = await handler.HandleAsync(new LoginUserCommand("none@example.com", "Secret1"));

        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.InvalidCredentials, result.Error);
    }

    [Fact]
    public async Task Login_Should_Fail_When_Password_Does_Not_Match()
    {
        var repository = new InMemoryUserRepository();
        repository.Seed(CreateUser("john@example.com", "hashed::Secret1"));
        var handler = new LoginUserCommandHandler(repository, new FakePasswordHasher(), new FakeJwtTokenService(), new FixedDateTimeProvider(DateTimeOffset.UtcNow), new FakeAuditService());

        var result = await handler.HandleAsync(new LoginUserCommand("john@example.com", "Wrong"));

        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.InvalidCredentials, result.Error);
    }

    [Fact]
    public async Task Login_Should_Return_Success_And_Placeholder_Token()
    {
        var repository = new InMemoryUserRepository();
        repository.Seed(CreateUser("john@example.com", "hashed::Secret1"));
        var handler = new LoginUserCommandHandler(repository, new FakePasswordHasher(), new FakeJwtTokenService(), new FixedDateTimeProvider(DateTimeOffset.UtcNow), new FakeAuditService());

        var result = await handler.HandleAsync(new LoginUserCommand("john@example.com", "Secret1"));

        Assert.True(result.IsSuccess);
        Assert.Equal("placeholder-token", result.Value.AccessToken);
    }

    [Fact]
    public async Task ChangeEmail_Should_Fail_When_User_Missing()
    {
        var handler = new ChangeUserEmailCommandHandler(new InMemoryUserRepository(), new FixedDateTimeProvider(DateTimeOffset.UtcNow));
        var result = await handler.HandleAsync(new ChangeUserEmailCommand(Guid.NewGuid(), "new@example.com"));

        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task ChangeEmail_Should_Reject_Invalid_Email()
    {
        var repository = new InMemoryUserRepository();
        var user = CreateUser("john@example.com");
        repository.Seed(user);
        var handler = new ChangeUserEmailCommandHandler(repository, new FixedDateTimeProvider(DateTimeOffset.UtcNow));

        var result = await handler.HandleAsync(new ChangeUserEmailCommand(user.Id.Value, "invalid"));

        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.EmailInvalid, result.Error);
    }

    [Fact]
    public async Task ChangeEmail_Should_Persist_Changed_Email()
    {
        var repository = new InMemoryUserRepository();
        var user = CreateUser("john@example.com");
        repository.Seed(user);
        var handler = new ChangeUserEmailCommandHandler(repository, new FixedDateTimeProvider(DateTimeOffset.UtcNow));

        var result = await handler.HandleAsync(new ChangeUserEmailCommand(user.Id.Value, "new@example.com"));

        Assert.True(result.IsSuccess);
        Assert.Equal("new@example.com", repository.Users.Single().Email.Value);
    }

    [Fact]
    public async Task ChangePassword_Should_Fail_When_User_Missing()
    {
        var handler = new ChangeUserPasswordCommandHandler(new InMemoryUserRepository(), new FixedDateTimeProvider(DateTimeOffset.UtcNow), new FakePasswordHasher());
        var result = await handler.HandleAsync(new ChangeUserPasswordCommand(Guid.NewGuid(), "Secret1"));

        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task ChangePassword_Should_Persist_Changed_Hash()
    {
        var repository = new InMemoryUserRepository();
        var user = CreateUser("john@example.com", "hashed::old");
        repository.Seed(user);
        var handler = new ChangeUserPasswordCommandHandler(repository, new FixedDateTimeProvider(DateTimeOffset.UtcNow), new FakePasswordHasher());

        var result = await handler.HandleAsync(new ChangeUserPasswordCommand(user.Id.Value, "new"));

        Assert.True(result.IsSuccess);
        Assert.Equal("hashed::new", repository.Users.Single().PasswordHash.Value);
    }

    [Fact]
    public async Task ChangePassword_Should_Return_Error_When_Hashed_Value_Is_Invalid()
    {
        var repository = new InMemoryUserRepository();
        var user = CreateUser("john@example.com", "hashed::old");
        repository.Seed(user);
        var handler = new ChangeUserPasswordCommandHandler(repository, new FixedDateTimeProvider(DateTimeOffset.UtcNow), new EmptyHasher());

        var result = await handler.HandleAsync(new ChangeUserPasswordCommand(user.Id.Value, "new"));

        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.PasswordHashEmpty, result.Error);
    }

    private static User CreateUser(string email, string passwordHash = "hashed::Secret1") =>
        User.Register(UserId.New().Value, Email.Create(email).Value, PasswordHash.Create(passwordHash).Value, UserDisplayName.Create("John").Value, DateTimeOffset.UtcNow);

    private sealed class EmptyHasher : Mavrynt.BuildingBlocks.Application.Abstractions.IPasswordHasher
    {
        public string HashPassword(string password) => string.Empty;
        public bool VerifyPassword(string password, string passwordHash) => false;
    }
}

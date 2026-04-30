using Mavrynt.Modules.Users.Application.Commands;
using Mavrynt.Modules.Users.Application.Tests.Fakes;
using Mavrynt.Modules.Users.Domain.Entities;
using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.ValueObjects;
using Xunit;

namespace Mavrynt.Modules.Users.Application.Tests;

public sealed class AssignUserRoleCommandHandlerTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    [Fact]
    public async Task AssignRole_Should_Set_Admin_Role()
    {
        var repo = new InMemoryUserRepository();
        var user = CreateUser("john@example.com");
        repo.Seed(user);
        var audit = new FakeAuditService();
        var handler = new AssignUserRoleCommandHandler(repo, new FixedDateTimeProvider(Now), audit);

        var result = await handler.HandleAsync(new AssignUserRoleCommand(user.Id.Value, "Admin"));

        Assert.True(result.IsSuccess);
        Assert.Equal("Admin", result.Value.Role);
        Assert.Single(audit.Entries, e => e.EventType == "user_role_assigned");
    }

    [Fact]
    public async Task AssignRole_Should_Set_User_Role()
    {
        var repo = new InMemoryUserRepository();
        var user = CreateUser("john@example.com");
        repo.Seed(user);
        var handler = new AssignUserRoleCommandHandler(repo, new FixedDateTimeProvider(Now), new FakeAuditService());

        var result = await handler.HandleAsync(new AssignUserRoleCommand(user.Id.Value, "User"));

        Assert.True(result.IsSuccess);
        Assert.Equal("User", result.Value.Role);
    }

    [Fact]
    public async Task AssignRole_Should_Return_NotFound_When_User_Missing()
    {
        var handler = new AssignUserRoleCommandHandler(
            new InMemoryUserRepository(), new FixedDateTimeProvider(Now), new FakeAuditService());

        var result = await handler.HandleAsync(new AssignUserRoleCommand(Guid.NewGuid(), "Admin"));

        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task AssignRole_Should_Return_InvalidRole_For_Unknown_Role_String()
    {
        var repo = new InMemoryUserRepository();
        var user = CreateUser("john@example.com");
        repo.Seed(user);
        var handler = new AssignUserRoleCommandHandler(repo, new FixedDateTimeProvider(Now), new FakeAuditService());

        var result = await handler.HandleAsync(new AssignUserRoleCommand(user.Id.Value, "SuperAdmin"));

        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.InvalidRole, result.Error);
    }

    [Fact]
    public async Task AssignRole_Should_Write_Audit_Entry_After_Successful_Assignment()
    {
        var repo = new InMemoryUserRepository();
        var user = CreateUser("jane@example.com");
        repo.Seed(user);
        var audit = new FakeAuditService();
        var handler = new AssignUserRoleCommandHandler(repo, new FixedDateTimeProvider(Now), audit);

        await handler.HandleAsync(new AssignUserRoleCommand(user.Id.Value, "Admin"));

        Assert.Single(audit.Entries);
        Assert.Equal("user_role_assigned", audit.Entries[0].EventType);
    }

    private static User CreateUser(string email) =>
        User.Register(
            UserId.New().Value,
            Email.Create(email).Value,
            PasswordHash.Create("hashed::pass").Value,
            UserDisplayName.Create("Test User").Value,
            Now);
}

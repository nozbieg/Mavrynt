using Mavrynt.Modules.Users.Domain.Entities;
using Mavrynt.Modules.Users.Domain.Enums;
using Mavrynt.Modules.Users.Domain.Events;
using Mavrynt.Modules.Users.Domain.ValueObjects;
using Xunit;

namespace Mavrynt.Modules.Users.Domain.Tests;

public sealed class UserAggregateTests
{
    [Fact]
    public void Register_Should_Create_Active_User_With_Expected_Data_And_Event()
    {
        var userId = UserId.New().Value;
        var email = Email.Create("john@example.com").Value;
        var passwordHash = PasswordHash.Create("hash").Value;
        var displayName = UserDisplayName.Create("John").Value;

        var user = User.Register(userId, email, passwordHash, displayName, DateTimeOffset.UtcNow);

        Assert.Equal(UserStatus.Active, user.Status);
        Assert.Equal(email, user.Email);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(displayName, user.DisplayName);
        Assert.Single(user.DomainEvents.OfType<UserRegisteredDomainEvent>());
    }

    [Fact]
    public void ChangeEmail_Should_Update_Email_And_Emit_Event()
    {
        var user = CreateUser();
        var newEmail = Email.Create("next@example.com").Value;

        var result = user.ChangeEmail(newEmail, DateTimeOffset.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Equal(newEmail, user.Email);
        Assert.NotEmpty(user.DomainEvents.OfType<UserEmailChangedDomainEvent>());
    }

    [Fact]
    public void ChangePasswordHash_Should_Update_Password_And_Emit_Event()
    {
        var user = CreateUser();
        var hash = PasswordHash.Create("new-hash").Value;

        var result = user.ChangePasswordHash(hash, DateTimeOffset.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Equal(hash, user.PasswordHash);
        Assert.NotEmpty(user.DomainEvents.OfType<UserPasswordChangedDomainEvent>());
    }

    [Fact]
    public void ChangeDisplayName_Should_Update_DisplayName()
    {
        var user = CreateUser();
        var displayName = UserDisplayName.Create("Display").Value;

        var result = user.ChangeDisplayName(displayName, DateTimeOffset.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Equal(displayName, user.DisplayName);
    }

    [Fact]
    public void Activate_And_Deactivate_Should_Update_Status()
    {
        var user = CreateUser();

        user.Deactivate(DateTimeOffset.UtcNow);
        Assert.Equal(UserStatus.Inactive, user.Status);

        user.Activate(DateTimeOffset.UtcNow);
        Assert.Equal(UserStatus.Active, user.Status);
    }

    private static User CreateUser() =>
        User.Register(
            UserId.New().Value,
            Email.Create("initial@example.com").Value,
            PasswordHash.Create("hash").Value,
            UserDisplayName.Create("Initial").Value,
            DateTimeOffset.UtcNow);
}

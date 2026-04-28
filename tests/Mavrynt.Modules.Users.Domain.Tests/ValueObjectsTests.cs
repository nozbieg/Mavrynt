using Mavrynt.Modules.Users.Domain.Errors;
using Mavrynt.Modules.Users.Domain.ValueObjects;

namespace Mavrynt.Modules.Users.Domain.Tests;

public sealed class ValueObjectsTests
{
    [Fact]
    public void Email_Create_Should_Accept_Valid_Email_And_Normalize()
    {
        var result = Email.Create("  USER@Example.COM ");
        Assert.True(result.IsSuccess);
        Assert.Equal("user@example.com", result.Value.Value);
    }

    [Fact]
    public void Email_Create_Should_Reject_Invalid_Email()
    {
        var result = Email.Create("invalid");
        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.EmailInvalid, result.Error);
    }

    [Fact]
    public void PasswordHash_Create_Should_Reject_Empty()
    {
        var result = PasswordHash.Create(" ");
        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.PasswordHashEmpty, result.Error);
    }

    [Fact]
    public void PasswordHash_Create_Should_Accept_Valid_Value()
    {
        var result = PasswordHash.Create("hashed-value");
        Assert.True(result.IsSuccess);
        Assert.Equal("hashed-value", result.Value.Value);
    }

    [Fact]
    public void DisplayName_Create_Should_Reject_Empty()
    {
        var result = UserDisplayName.Create(string.Empty);
        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.DisplayNameEmpty, result.Error);
    }

    [Fact]
    public void DisplayName_Create_Should_Reject_Too_Long_Value()
    {
        var value = new string('a', UserDisplayName.MaxLength + 1);
        var result = UserDisplayName.Create(value);
        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.DisplayNameTooLong, result.Error);
    }

    [Fact]
    public void DisplayName_Create_Should_Accept_Valid_Value()
    {
        var result = UserDisplayName.Create("  Alice  ");
        Assert.True(result.IsSuccess);
        Assert.Equal("Alice", result.Value.Value);
    }

    [Fact]
    public void UserId_From_Should_Reject_Empty_Guid()
    {
        var result = UserId.From(Guid.Empty);
        Assert.True(result.IsFailure);
        Assert.Same(UserErrors.InvalidUserId, result.Error);
    }

    [Fact]
    public void UserId_New_Should_Create_Non_Empty_Value()
    {
        var result = UserId.New();
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Value);
    }
}

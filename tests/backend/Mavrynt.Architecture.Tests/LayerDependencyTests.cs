using NetArchTest.Rules;
using Xunit;

namespace Mavrynt.Architecture.Tests;

public sealed class LayerDependencyTests
{
    [Fact]
    public void Users_Domain_Should_Not_Depend_On_Forbidden_Assemblies()
    {
        var result = Types.InAssembly(typeof(Mavrynt.Modules.Users.Domain.Entities.User).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Mavrynt.Modules.Users.Application",
                "Mavrynt.Modules.Users.Infrastructure",
                "Mavrynt.Api",
                "Mavrynt.AdminApp",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Npgsql")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }

    [Fact]
    public void Users_Application_Should_Not_Depend_On_Forbidden_Assemblies()
    {
        var result = Types.InAssembly(typeof(Mavrynt.Modules.Users.Application.Commands.RegisterUserCommand).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Mavrynt.Modules.Users.Infrastructure",
                "Mavrynt.Api",
                "Mavrynt.AdminApp",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Npgsql")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }

    [Fact]
    public void Users_Infrastructure_Should_Not_Depend_On_Hosts()
    {
        var result = Types.InAssembly(typeof(Mavrynt.Modules.Users.Infrastructure.Persistence.UsersDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Mavrynt.Api", "Mavrynt.AdminApp", "Mavrynt.AppHost")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }
}

internal static class NetArchResultExtensions
{
    public static string GetFailingTypes(this TestResult result) =>
        string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>());
}

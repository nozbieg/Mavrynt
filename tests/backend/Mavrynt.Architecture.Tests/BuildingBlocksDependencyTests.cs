using NetArchTest.Rules;

namespace Mavrynt.Architecture.Tests;

public sealed class BuildingBlocksDependencyTests
{
    [Fact]
    public void BuildingBlocks_Domain_Should_Be_Framework_Free()
    {
        var result = Types.InAssembly(typeof(Mavrynt.BuildingBlocks.Domain.Results.Result).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore", "Npgsql", "System.Net.Http")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }

    [Fact]
    public void BuildingBlocks_Application_Should_Not_Depend_On_Infrastructure_Or_Host_Frameworks()
    {
        var result = Types.InAssembly(typeof(Mavrynt.BuildingBlocks.Application.Messaging.IMediator).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Mavrynt.BuildingBlocks.Infrastructure",
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
    public void BuildingBlocks_Contracts_Should_Not_Depend_On_Host_Or_Infrastructure_Projects()
    {
        var result = Types.InAssembly(typeof(Mavrynt.BuildingBlocksContracts.Events.IntegrationEvent).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Mavrynt.Api",
                "Mavrynt.AdminApp",
                "Mavrynt.AppHost",
                "Mavrynt.BuildingBlocks.Infrastructure",
                "Mavrynt.Modules.Users.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }
}

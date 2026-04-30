using NetArchTest.Rules;
using Xunit;

namespace Mavrynt.Architecture.Tests;

public sealed class NotificationsLayerDependencyTests
{
    [Fact]
    public void Notifications_Domain_Should_Not_Depend_On_Forbidden_Assemblies()
    {
        var result = Types.InAssembly(typeof(Mavrynt.Modules.Notifications.Domain.Entities.SmtpSettings).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Mavrynt.Modules.Notifications.Application",
                "Mavrynt.Modules.Notifications.Infrastructure",
                "Mavrynt.Api",
                "Mavrynt.AdminApp",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Npgsql")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }

    [Fact]
    public void Notifications_Application_Should_Not_Depend_On_Forbidden_Assemblies()
    {
        var result = Types.InAssembly(typeof(Mavrynt.Modules.Notifications.Application.Abstractions.IEmailNotificationService).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Mavrynt.Modules.Notifications.Infrastructure",
                "Mavrynt.Api",
                "Mavrynt.AdminApp",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Npgsql")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }

    [Fact]
    public void Notifications_Infrastructure_Should_Not_Depend_On_Hosts()
    {
        var result = Types.InAssembly(typeof(Mavrynt.Modules.Notifications.Infrastructure.Persistence.NotificationsDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Mavrynt.Api", "Mavrynt.AdminApp", "Mavrynt.AppHost")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }
}

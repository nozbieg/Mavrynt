using NetArchTest.Rules;
using Xunit;

namespace Mavrynt.Architecture.Tests;

public sealed class Phase1LayerDependencyTests
{
    // ── FeatureManagement ──────────────────────────────────────────────────────

    [Fact]
    public void FeatureManagement_Domain_Should_Not_Depend_On_Forbidden_Assemblies()
    {
        var result = Types.InAssembly(typeof(Mavrynt.Modules.FeatureManagement.Domain.Entities.FeatureFlag).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Mavrynt.Modules.FeatureManagement.Application",
                "Mavrynt.Modules.FeatureManagement.Infrastructure",
                "Mavrynt.Api",
                "Mavrynt.AdminApp",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Npgsql")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }

    [Fact]
    public void FeatureManagement_Application_Should_Not_Depend_On_Forbidden_Assemblies()
    {
        var result = Types.InAssembly(typeof(Mavrynt.Modules.FeatureManagement.Application.Commands.CreateFeatureFlagCommand).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Mavrynt.Modules.FeatureManagement.Infrastructure",
                "Mavrynt.Api",
                "Mavrynt.AdminApp",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Npgsql")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }

    [Fact]
    public void FeatureManagement_Infrastructure_Should_Not_Depend_On_Hosts()
    {
        var result = Types.InAssembly(typeof(Mavrynt.Modules.FeatureManagement.Infrastructure.Persistence.FeatureManagementDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Mavrynt.Api", "Mavrynt.AdminApp", "Mavrynt.AppHost")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }

    // ── Audit ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Audit_Domain_Should_Not_Depend_On_Forbidden_Assemblies()
    {
        var result = Types.InAssembly(typeof(Mavrynt.Modules.Audit.Domain.Entities.AuditLogEntry).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Mavrynt.Modules.Audit.Application",
                "Mavrynt.Modules.Audit.Infrastructure",
                "Mavrynt.Api",
                "Mavrynt.AdminApp",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Npgsql")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }

    [Fact]
    public void Audit_Application_Should_Not_Depend_On_Forbidden_Assemblies()
    {
        var result = Types.InAssembly(typeof(Mavrynt.Modules.Audit.Application.Abstractions.IAuditLogWriter).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Mavrynt.Modules.Audit.Infrastructure",
                "Mavrynt.Api",
                "Mavrynt.AdminApp",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Npgsql")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }

    [Fact]
    public void Audit_Infrastructure_Should_Not_Depend_On_Hosts()
    {
        var result = Types.InAssembly(typeof(Mavrynt.Modules.Audit.Infrastructure.Persistence.AuditDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Mavrynt.Api", "Mavrynt.AdminApp", "Mavrynt.AppHost")
            .GetResult();

        Assert.True(result.IsSuccessful, result.GetFailingTypes());
    }
}

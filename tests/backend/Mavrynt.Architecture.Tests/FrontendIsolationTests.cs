namespace Mavrynt.Architecture.Tests;

public sealed class FrontendIsolationTests
{
    private static readonly string[] FrontendProjects =
    [
        "src/frontend/Mavrynt.Web.Admin/Mavrynt.Web.Admin.csproj",
        "src/frontend/Mavrynt.Web.App/Mavrynt.Web.App.csproj",
        "src/frontend/Mavrynt.Web.Landing/Mavrynt.Web.Landing.csproj"
    ];

    [Theory]
    [MemberData(nameof(GetFrontendProjects))]
    public void Frontend_Projects_Should_Not_Reference_Backend_Modules(string frontendCsprojPath)
    {
        var references = ArchitectureTestPaths.GetProjectReferences(frontendCsprojPath);

        Assert.All(references, path =>
        {
            Assert.DoesNotContain("Mavrynt.Modules.Users.Domain", path, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Mavrynt.Modules.Users.Application", path, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Mavrynt.Modules.Users.Infrastructure", path, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Mavrynt.BuildingBlocks.Domain", path, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Mavrynt.BuildingBlocks.Application", path, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Mavrynt.BuildingBlocks.Infrastructure", path, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public void Landing_Should_Not_Reference_Backend_Runtime_Hosts_Or_Modules()
    {
        var references = ArchitectureTestPaths.GetProjectReferences("src/frontend/Mavrynt.Web.Landing/Mavrynt.Web.Landing.csproj");

        Assert.All(references, path =>
        {
            Assert.DoesNotContain("Mavrynt.Api", path, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Mavrynt.AdminApp", path, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Mavrynt.Modules.", path, StringComparison.OrdinalIgnoreCase);
        });
    }

    public static IEnumerable<object[]> GetFrontendProjects() =>
        FrontendProjects.Select(path => new object[] { path });
}

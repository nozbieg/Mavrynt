namespace Mavrynt.Architecture.Tests;

public sealed class HostDependencyTests
{
    [Theory]
    [InlineData("src/backend/Mavrynt.Api/Mavrynt.Api.csproj")]
    [InlineData("src/backend/Mavrynt.AdminApp/Mavrynt.AdminApp.csproj")]
    public void Hosts_Should_Not_Reference_Test_Projects(string hostCsprojPath)
    {
        var references = ArchitectureTestPaths.GetProjectReferences(hostCsprojPath);

        Assert.All(references, path =>
            Assert.DoesNotContain("Tests", Path.GetFileName(path), StringComparison.OrdinalIgnoreCase));
    }
}

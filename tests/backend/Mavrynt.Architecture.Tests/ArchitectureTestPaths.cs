using System.Xml.Linq;

namespace Mavrynt.Architecture.Tests;

internal static class ArchitectureTestPaths
{
    private static readonly string RepositoryRoot = ResolveRepositoryRoot();

    public static string RepoFile(string relativePath) => Path.Combine(RepositoryRoot, relativePath);

    public static IReadOnlyCollection<string> GetProjectReferences(string relativeCsprojPath)
    {
        var csprojPath = RepoFile(relativeCsprojPath);
        var doc = XDocument.Load(csprojPath);

        var projectDirectory = Path.GetDirectoryName(csprojPath)!;

        return doc
            .Descendants("ProjectReference")
            .Select(x => x.Attribute("Include")?.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => Path.GetFullPath(Path.Combine(projectDirectory, x!)))
            .ToArray();
    }

    private static string ResolveRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Mavrynt.sln")))
                return current.FullName;

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not resolve repository root for architecture tests.");
    }
}

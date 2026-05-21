using clsoc;
using Xunit;

namespace clsoc.Tests;

public sealed class FileScannerTests : IDisposable
{
    private readonly string testRoot;

    public FileScannerTests()
    {
        this.testRoot = Path.Combine(Path.GetTempPath(), "clsoc-filescanner-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(this.testRoot);
    }

    [Fact]
    public void FindFiles_ShouldIgnoreDefaultExcludedDirectories()
    {
        WriteFile("Program.cs", "class Program { }");
        WriteFile(Path.Combine("bin", "Debug", "Generated.cs"), "class Generated { }");
        WriteFile(Path.Combine("obj", "Debug", "Generated.cs"), "class Generated { }");
        WriteFile(Path.Combine(".git", "hooks", "Hook.cs"), "class Hook { }");
        FileScanner scanner = new();

        IReadOnlyList<string> files = scanner.FindFiles(this.testRoot, "cs");

        string file = Assert.Single(files);
        Assert.EndsWith("Program.cs", file, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FindFiles_WhenDefaultExcludesAreDisabled_ShouldIncludeNormallyIgnoredDirectories()
    {
        WriteFile("Program.cs", "class Program { }");
        WriteFile(Path.Combine("bin", "Debug", "Generated.cs"), "class Generated { }");
        FileScanner scanner = new();
        FileScanOptions options = new(useDefaultExcludes: false, excludedDirectoryNames: Array.Empty<string>());

        IReadOnlyList<string> files = scanner.FindFiles(this.testRoot, "cs", options);

        Assert.Equal(2, files.Count);
    }

    [Fact]
    public void FindFiles_WhenCustomExcludeIsProvided_ShouldSkipThatDirectory()
    {
        WriteFile("Program.cs", "class Program { }");
        WriteFile(Path.Combine("generated", "Generated.cs"), "class Generated { }");
        FileScanner scanner = new();
        FileScanOptions options = new(useDefaultExcludes: true, excludedDirectoryNames: new[] { "generated" });

        IReadOnlyList<string> files = scanner.FindFiles(this.testRoot, "cs", options);

        string file = Assert.Single(files);
        Assert.EndsWith("Program.cs", file, StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        if (Directory.Exists(this.testRoot))
        {
            Directory.Delete(this.testRoot, recursive: true);
        }
    }

    private void WriteFile(string relativePath, params string[] lines)
    {
        string path = Path.Combine(this.testRoot, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllLines(path, lines);
    }
}

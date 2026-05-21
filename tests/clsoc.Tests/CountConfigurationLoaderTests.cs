using clsoc;
using Xunit;

namespace clsoc.Tests;

public sealed class CountConfigurationLoaderTests : IDisposable
{
    private readonly string testRoot;

    public CountConfigurationLoaderTests()
    {
        this.testRoot = Path.Combine(Path.GetTempPath(), "clsoc-configuration-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(this.testRoot);
    }

    [Fact]
    public void Load_ShouldReadConfigurationFile()
    {
        string configPath = WriteConfig(
            """
            {
              "rootPath": "src",
              "languages": ["csharp", "xml"],
              "extensions": ["cs", "xaml"],
              "excludeDirectories": ["generated", "temp"],
              "useDefaultExcludes": false,
              "outputFormat": "json",
              "outputPath": "report.json"
            }
            """);
        CountConfigurationLoader loader = new();

        CountConfiguration configuration = loader.Load(configPath);

        Assert.Equal("src", configuration.RootPath);
        Assert.Equal(new[] { "csharp", "xml" }, configuration.Languages);
        Assert.Equal(new[] { "cs", "xaml" }, configuration.Extensions);
        Assert.Equal(new[] { "generated", "temp" }, configuration.ExcludeDirectories);
        Assert.False(configuration.UseDefaultExcludes);
        Assert.Equal("json", configuration.OutputFormat);
        Assert.Equal("report.json", configuration.OutputPath);
    }

    [Fact]
    public void Load_ShouldAllowCommentsAndTrailingCommas()
    {
        string configPath = WriteConfig(
            """
            {
              // Project-specific report format.
              "outputFormat": "markdown",
            }
            """);
        CountConfigurationLoader loader = new();

        CountConfiguration configuration = loader.Load(configPath);

        Assert.Equal("markdown", configuration.OutputFormat);
    }

    [Fact]
    public void LoadIfExists_WhenDefaultConfigExistsInRootPath_ShouldLoadIt()
    {
        string configPath = Path.Combine(this.testRoot, CountConfigurationLoader.DefaultFileName);
        File.WriteAllText(configPath, "{ \"outputFormat\": \"csv\" }");
        CountConfigurationLoader loader = new();

        CountConfiguration? configuration = loader.LoadIfExists(this.testRoot, explicitConfigPath: null);

        Assert.NotNull(configuration);
        Assert.Equal("csv", configuration.OutputFormat);
    }

    [Fact]
    public void LoadIfExists_WhenConfigDoesNotExist_ShouldReturnNull()
    {
        CountConfigurationLoader loader = new();

        CountConfiguration? configuration = loader.LoadIfExists(this.testRoot, explicitConfigPath: null);

        Assert.Null(configuration);
    }

    [Fact]
    public void ResolveConfigPath_WhenExplicitPathIsProvided_ShouldReturnFullPath()
    {
        string explicitPath = Path.Combine(this.testRoot, "custom.json");

        string? resolved = CountConfigurationLoader.ResolveConfigPath(this.testRoot, explicitPath);

        Assert.Equal(Path.GetFullPath(explicitPath), resolved);
    }

    public void Dispose()
    {
        if (Directory.Exists(this.testRoot))
        {
            Directory.Delete(this.testRoot, recursive: true);
        }
    }

    private string WriteConfig(string content)
    {
        string path = Path.Combine(this.testRoot, "config.json");
        File.WriteAllText(path, content);
        return path;
    }
}

using clsoc;
using Xunit;

namespace clsoc.Tests;

public sealed class ProjectCounterTests : IDisposable
{
    private readonly string testRoot;

    public ProjectCounterTests()
    {
        this.testRoot = Path.Combine(Path.GetTempPath(), "clsoc-projectcounter-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(this.testRoot);
    }

    [Fact]
    public void CountByLanguage_WhenProjectContainsMultipleLanguages_ShouldReturnSeparateResults()
    {
        WriteFile("Program.cs", "class Program", "{", "}");
        WriteFile("View.xaml", "<!-- comment -->", "<Grid />");
        ProjectCounter counter = new();

        IReadOnlyList<LanguageCountResult> results = counter.CountByLanguage(
            this.testRoot,
            new[] { LanguageRegistry.CSharp, LanguageRegistry.Xml });

        LanguageCountResult csharp = Assert.Single(results, result => result.Language.Id == "csharp");
        LanguageCountResult xml = Assert.Single(results, result => result.Language.Id == "xml");
        Assert.Equal(1, csharp.Result.Files);
        Assert.Equal(3, csharp.Result.CodeLines);
        Assert.Equal(1, xml.Result.Files);
        Assert.Equal(1, xml.Result.CodeLines);
        Assert.Equal(1, xml.Result.CommentLines);
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

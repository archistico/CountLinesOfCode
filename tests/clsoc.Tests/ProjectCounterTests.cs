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
        Assert.Single(csharp.FileResults);
        Assert.Equal("Program.cs", Path.GetFileName(csharp.LargestFile!.FilePath));
        Assert.Equal(3, csharp.LargestFile.Result.TotalLines);
        Assert.Equal(1, xml.Result.Files);
        Assert.Equal(1, xml.Result.CodeLines);
        Assert.Equal(1, xml.Result.CommentLines);
    }


    [Fact]
    public void CountByLanguage_ShouldRespectScanOptions()
    {
        WriteFile("Program.cs", "class Program", "{", "}");
        WriteFile(Path.Combine("generated", "Generated.cs"), "class Generated", "{", "}");
        ProjectCounter counter = new();
        FileScanOptions options = new(useDefaultExcludes: true, excludedDirectoryNames: new[] { "generated" });

        IReadOnlyList<LanguageCountResult> results = counter.CountByLanguage(
            this.testRoot,
            new[] { LanguageRegistry.CSharp },
            options);

        LanguageCountResult csharp = Assert.Single(results, result => result.Language.Id == "csharp");
        Assert.Equal(1, csharp.Result.Files);
        Assert.Equal(3, csharp.Result.CodeLines);
    }

    [Fact]
    public void CountByLanguage_ShouldTrackLargestFileAndAverages()
    {
        WriteFile("Small.cs", "class Small");
        WriteFile("Large.cs", "class Large", "{", "}", "// comment");
        ProjectCounter counter = new();

        IReadOnlyList<LanguageCountResult> results = counter.CountByLanguage(
            this.testRoot,
            new[] { LanguageRegistry.CSharp });

        LanguageCountResult csharp = Assert.Single(results, result => result.Language.Id == "csharp");
        Assert.Equal(2, csharp.FileResults.Count);
        Assert.Equal("Large.cs", Path.GetFileName(csharp.LargestFile!.FilePath));
        Assert.Equal(2.5, csharp.AverageLinesPerFile);
        Assert.Equal(0.2, csharp.CommentRatio);
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

using clsoc;
using Xunit;

namespace clsoc.Tests;

public sealed class LineCounterTests : IDisposable
{
    private readonly string testRoot;

    public LineCounterTests()
    {
        this.testRoot = Path.Combine(Path.GetTempPath(), "clsoc-linecounter-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(this.testRoot);
    }

    [Fact]
    public void CountFile_WhenFileContainsSimpleCode_ShouldReturnSingleFileResult()
    {
        string file = this.WriteFile("Program.cs", "class Program", "{", "}");
        LineCounter counter = new();

        LineCountResult result = counter.CountFile(file);

        Assert.Equal(1, result.Files);
        Assert.Equal(3, result.TotalLines);
        Assert.Equal(3, result.CodeLines);
        Assert.Equal(0, result.BlankLines);
        Assert.Equal(0, result.CommentLines);
    }

    [Fact]
    public void CountFile_WhenFileContainsBlankAndCommentLines_ShouldReturnSeparatedCounts()
    {
        string file = this.WriteFile("Program.cs", "// comment", string.Empty, "class Program");
        LineCounter counter = new();

        LineCountResult result = counter.CountFile(file);

        Assert.Equal(3, result.TotalLines);
        Assert.Equal(1, result.CodeLines);
        Assert.Equal(1, result.BlankLines);
        Assert.Equal(1, result.CommentLines);
    }


    [Fact]
    public void CountFile_WhenBlockCommentStartsAndEndsOnSameLine_ShouldCountCommentLine()
    {
        string file = this.WriteFile("Program.cs", "/* comment */", "class Program");
        LineCounter counter = new();

        LineCountResult result = counter.CountFile(file);

        Assert.Equal(2, result.TotalLines);
        Assert.Equal(1, result.CodeLines);
        Assert.Equal(0, result.BlankLines);
        Assert.Equal(1, result.CommentLines);
    }

    [Fact]
    public void CountFile_WhenLineContainsCodeBeforeLineComment_ShouldCountAsCodeLine()
    {
        string file = this.WriteFile("Program.cs", "int value = 10; // inline comment");
        LineCounter counter = new();

        LineCountResult result = counter.CountFile(file);

        Assert.Equal(1, result.TotalLines);
        Assert.Equal(1, result.CodeLines);
        Assert.Equal(0, result.CommentLines);
        Assert.Equal(0, result.BlankLines);
    }

    [Fact]
    public void CountFile_WhenLineContainsInlineBlockCommentBetweenCode_ShouldCountAsCodeLine()
    {
        string file = this.WriteFile("Program.cs", "int value = /* inline comment */ 10;");
        LineCounter counter = new();

        LineCountResult result = counter.CountFile(file);

        Assert.Equal(1, result.TotalLines);
        Assert.Equal(1, result.CodeLines);
        Assert.Equal(0, result.CommentLines);
        Assert.Equal(0, result.BlankLines);
    }

    [Fact]
    public void CountFile_WhenCodeFollowsClosedBlockComment_ShouldCountAsCodeLine()
    {
        string file = this.WriteFile("Program.cs", "/* comment */ int value = 10;");
        LineCounter counter = new();

        LineCountResult result = counter.CountFile(file);

        Assert.Equal(1, result.TotalLines);
        Assert.Equal(1, result.CodeLines);
        Assert.Equal(0, result.CommentLines);
        Assert.Equal(0, result.BlankLines);
    }

    [Fact]
    public void CountFile_WhenCommentMarkersAreInsideStrings_ShouldNotCountComments()
    {
        string file = this.WriteFile(
            "Program.cs",
            "string slash = \"// not a comment\";",
            "string block = \"/* not a comment */\";");
        LineCounter counter = new();

        LineCountResult result = counter.CountFile(file);

        Assert.Equal(2, result.TotalLines);
        Assert.Equal(2, result.CodeLines);
        Assert.Equal(0, result.CommentLines);
        Assert.Equal(0, result.BlankLines);
    }

    [Fact]
    public void CountFile_WhenBlockCommentStartsAfterCodeAndEndsOnLaterLine_ShouldClassifyOnlyPureCommentLinesAsComments()
    {
        string file = this.WriteFile(
            "Program.cs",
            "int before = 1; /* start",
            "middle",
            "end */",
            "int after = 2;");
        LineCounter counter = new();

        LineCountResult result = counter.CountFile(file);

        Assert.Equal(4, result.TotalLines);
        Assert.Equal(2, result.CodeLines);
        Assert.Equal(2, result.CommentLines);
        Assert.Equal(0, result.BlankLines);
    }

    public void Dispose()
    {
        if (Directory.Exists(this.testRoot))
        {
            Directory.Delete(this.testRoot, recursive: true);
        }
    }

    private string WriteFile(string relativePath, params string[] lines)
    {
        string path = Path.Combine(this.testRoot, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllLines(path, lines);
        return path;
    }
}

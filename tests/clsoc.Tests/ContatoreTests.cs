using clsoc;
using Xunit;

namespace clsoc.Tests;

public sealed class ContatoreTests : IDisposable
{
    private readonly string testRoot;

    public ContatoreTests()
    {
        this.testRoot = Path.Combine(Path.GetTempPath(), "clsoc-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(this.testRoot);
    }

    [Fact]
    public void ConteggiaLinee_WhenNoMatchingFiles_ShouldReturnZeroCounts()
    {
        Contatore contatore = new();

        contatore.ConteggiaLinee("cs", this.testRoot);

        Assert.Equal(0, contatore.numberOfFile);
        Assert.Equal(0, contatore.linesOfcodeTotal);
        Assert.Equal(0, contatore.linesOfCode);
        Assert.Equal(0, contatore.linesOfcodeEmpty);
        Assert.Equal(0, contatore.linesOfcodeComment);
    }

    [Fact]
    public void ConteggiaLinee_WhenFileContainsOnlyCode_ShouldCountCodeLines()
    {
        WriteFile("Program.cs", "class Program", "{", "}");
        Contatore contatore = new();

        contatore.ConteggiaLinee("cs", this.testRoot);

        Assert.Equal(1, contatore.numberOfFile);
        Assert.Equal(3, contatore.linesOfcodeTotal);
        Assert.Equal(3, contatore.linesOfCode);
        Assert.Equal(0, contatore.linesOfcodeEmpty);
        Assert.Equal(0, contatore.linesOfcodeComment);
    }

    [Fact]
    public void ConteggiaLinee_WhenFileContainsBlankLines_ShouldCountBlankLines()
    {
        WriteFile("Program.cs", "class Program", string.Empty, "{", "   ", "}");
        Contatore contatore = new();

        contatore.ConteggiaLinee("cs", this.testRoot);

        Assert.Equal(1, contatore.numberOfFile);
        Assert.Equal(5, contatore.linesOfcodeTotal);
        Assert.Equal(3, contatore.linesOfCode);
        Assert.Equal(2, contatore.linesOfcodeEmpty);
        Assert.Equal(0, contatore.linesOfcodeComment);
    }

    [Fact]
    public void ConteggiaLinee_WhenFileContainsLineComments_ShouldCountCommentLines()
    {
        WriteFile("Program.cs", "// commento", "' commento stile VB", "class Program");
        Contatore contatore = new();

        contatore.ConteggiaLinee("cs", this.testRoot);

        Assert.Equal(1, contatore.numberOfFile);
        Assert.Equal(3, contatore.linesOfcodeTotal);
        Assert.Equal(1, contatore.linesOfCode);
        Assert.Equal(0, contatore.linesOfcodeEmpty);
        Assert.Equal(2, contatore.linesOfcodeComment);
    }

    [Fact]
    public void ConteggiaLinee_WhenFileContainsSimpleBlockComment_ShouldCountCommentLines()
    {
        WriteFile("Program.cs", "/*", "commento", "*/", "class Program");
        Contatore contatore = new();

        contatore.ConteggiaLinee("cs", this.testRoot);

        Assert.Equal(1, contatore.numberOfFile);
        Assert.Equal(4, contatore.linesOfcodeTotal);
        Assert.Equal(1, contatore.linesOfCode);
        Assert.Equal(0, contatore.linesOfcodeEmpty);
        Assert.Equal(3, contatore.linesOfcodeComment);
    }

    [Fact]
    public void ConteggiaLinee_ShouldSearchSubdirectories()
    {
        Directory.CreateDirectory(Path.Combine(this.testRoot, "src"));
        WriteFile("Root.cs", "class Root");
        WriteFile(Path.Combine("src", "Nested.cs"), "class Nested");
        Contatore contatore = new();

        contatore.ConteggiaLinee(".cs", this.testRoot);

        Assert.Equal(2, contatore.numberOfFile);
        Assert.Equal(2, contatore.linesOfcodeTotal);
        Assert.Equal(2, contatore.linesOfCode);
    }

    [Fact]
    public void ConteggiaLinee_WhenRootPathDoesNotExist_ShouldThrowDirectoryNotFoundException()
    {
        Contatore contatore = new();
        string missingPath = Path.Combine(this.testRoot, "missing");

        Assert.Throws<DirectoryNotFoundException>(() => contatore.ConteggiaLinee("cs", missingPath));
    }

    [Fact]
    public void ConteggiaLinee_WhenCalledTwice_ShouldResetPreviousCounts()
    {
        WriteFile("Program.cs", "class Program");
        Contatore contatore = new();

        contatore.ConteggiaLinee("cs", this.testRoot);
        File.Delete(Path.Combine(this.testRoot, "Program.cs"));
        contatore.ConteggiaLinee("cs", this.testRoot);

        Assert.Equal(0, contatore.numberOfFile);
        Assert.Equal(0, contatore.linesOfcodeTotal);
        Assert.Equal(0, contatore.linesOfCode);
        Assert.Equal(0, contatore.linesOfcodeEmpty);
        Assert.Equal(0, contatore.linesOfcodeComment);
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

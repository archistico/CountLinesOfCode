using System.Text.Json;
using clsoc;
using Xunit;

namespace clsoc.Tests;

public sealed class CountReportFormatterTests
{
    [Fact]
    public void Format_Table_ShouldIncludeLanguageRowsAndTotal()
    {
        CountReportFormatter formatter = new();

        string report = formatter.Format(CreateResults(), ReportFormat.Table);

        Assert.Contains("Language", report);
        Assert.Contains("C#", report);
        Assert.Contains("XML/XAML", report);
        Assert.Contains("Total", report);
        Assert.Contains("Metrics", report);
        Assert.Contains("Comment ratio", report);
        Assert.Contains("Largest file", report);
        Assert.Contains("     2", report);
    }

    [Fact]
    public void Format_Json_ShouldProduceStructuredReport()
    {
        CountReportFormatter formatter = new();

        string report = formatter.Format(CreateResults(), ReportFormat.Json);

        using JsonDocument document = JsonDocument.Parse(report);
        JsonElement root = document.RootElement;
        Assert.Equal(2, root.GetProperty("languages").GetArrayLength());
        Assert.Equal(3, root.GetProperty("total").GetProperty("files").GetInt32());
        Assert.Equal(14, root.GetProperty("total").GetProperty("code").GetInt32());
        Assert.Equal(0.1905, root.GetProperty("total").GetProperty("metrics").GetProperty("commentRatio").GetDouble());
        Assert.Equal("src/Program.cs", root.GetProperty("total").GetProperty("metrics").GetProperty("largestFile").GetProperty("path").GetString());
    }

    [Fact]
    public void Format_Markdown_ShouldProduceMarkdownTable()
    {
        CountReportFormatter formatter = new();

        string report = formatter.Format(CreateResults(), ReportFormat.Markdown);

        Assert.Contains("| Language | Files | Code | Comments | Blank | Total |", report);
        Assert.Contains("| C# | 2 | 10 | 3 | 2 | 15 |", report);
        Assert.Contains("| **Total** | **3** | **14** | **4** | **3** | **21** |", report);
        Assert.Contains("## Metrics", report);
        Assert.Contains("- Average lines/file: 7", report);
    }

    [Fact]
    public void Format_Csv_ShouldProduceCsvRows()
    {
        CountReportFormatter formatter = new();

        string report = formatter.Format(CreateResults(), ReportFormat.Csv);

        Assert.Contains("Language,Files,Code,Comments,Blank,Total", report);
        Assert.Contains("C#,2,10,3,2,15", report);
        Assert.Contains("Total,3,14,4,3,21", report);
        Assert.Contains("Metric,Value", report);
        Assert.Contains("Code ratio,66.67 %", report);
    }

    [Fact]
    public void Format_ShouldSkipLanguagesWithoutFiles()
    {
        CountReportFormatter formatter = new();
        LanguageCountResult empty = new(LanguageRegistry.Python, LineCountResult.Empty);

        string report = formatter.Format(CreateResults().Concat(new[] { empty }).ToArray(), ReportFormat.Table);

        Assert.DoesNotContain("Python", report);
    }

    [Theory]
    [InlineData("table", ReportFormat.Table)]
    [InlineData("json", ReportFormat.Json)]
    [InlineData("markdown", ReportFormat.Markdown)]
    [InlineData("md", ReportFormat.Markdown)]
    [InlineData("csv", ReportFormat.Csv)]
    public void TryParse_WhenKnownFormatIsProvided_ShouldReturnExpectedFormat(string value, ReportFormat expected)
    {
        bool parsed = ReportFormatParser.TryParse(value, out ReportFormat actual);

        Assert.True(parsed);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryParse_WhenUnknownFormatIsProvided_ShouldReturnFalse()
    {
        bool parsed = ReportFormatParser.TryParse("unknown", out _);

        Assert.False(parsed);
    }

    private static IReadOnlyList<LanguageCountResult> CreateResults()
    {
        return new[]
        {
            new LanguageCountResult(
                LanguageRegistry.CSharp,
                new LineCountResult(2, 15, 10, 2, 3),
                new[]
                {
                    new FileCountResult("src/Program.cs", new LineCountResult(1, 10, 7, 1, 2)),
                    new FileCountResult("src/Other.cs", new LineCountResult(1, 5, 3, 1, 1))
                }),
            new LanguageCountResult(
                LanguageRegistry.Xml,
                new LineCountResult(1, 6, 4, 1, 1),
                new[]
                {
                    new FileCountResult("View.xaml", new LineCountResult(1, 6, 4, 1, 1))
                })
        };
    }
}

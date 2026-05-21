using System.Globalization;
using System.Text;
using System.Text.Json;

namespace clsoc;

public sealed class CountReportFormatter
{
    public string Format(IReadOnlyList<LanguageCountResult> results, ReportFormat format)
    {
        ArgumentNullException.ThrowIfNull(results);

        IReadOnlyList<LanguageCountResult> visibleResults = results
            .Where(result => result.Result.Files > 0)
            .ToArray();

        LineCountResult total = CalculateTotal(visibleResults);
        FileCountResult? largestFile = FindLargestFile(visibleResults);

        return format switch
        {
            ReportFormat.Table => FormatTable(visibleResults, total, largestFile),
            ReportFormat.Json => FormatJson(visibleResults, total, largestFile),
            ReportFormat.Markdown => FormatMarkdown(visibleResults, total, largestFile),
            ReportFormat.Csv => FormatCsv(visibleResults, total, largestFile),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported report format.")
        };
    }

    private static string FormatTable(
        IReadOnlyList<LanguageCountResult> results,
        LineCountResult total,
        FileCountResult? largestFile)
    {
        StringBuilder builder = new();
        builder.AppendLine("Language                 Files      Code  Comments     Blank     Total");
        builder.AppendLine("-----------------------------------------------------------------------");

        foreach (LanguageCountResult result in results)
        {
            builder.AppendLine(
                $"{result.Language.DisplayName,-22} {result.Result.Files,5} {result.Result.CodeLines,9} {result.Result.CommentLines,9} {result.Result.BlankLines,9} {result.Result.TotalLines,9}");
        }

        builder.AppendLine("-----------------------------------------------------------------------");
        builder.AppendLine(
            $"{"Total",-22} {total.Files,5} {total.CodeLines,9} {total.CommentLines,9} {total.BlankLines,9} {total.TotalLines,9}");
        builder.AppendLine();
        builder.AppendLine("Metrics");
        builder.AppendLine("-------");
        builder.AppendLine($"Comment ratio:        {FormatPercentage(CalculateRatio(total.CommentLines, total.TotalLines)),8}");
        builder.AppendLine($"Blank ratio:          {FormatPercentage(CalculateRatio(total.BlankLines, total.TotalLines)),8}");
        builder.AppendLine($"Code ratio:           {FormatPercentage(CalculateRatio(total.CodeLines, total.TotalLines)),8}");
        builder.AppendLine($"Average lines/file:   {FormatNumber(CalculateRatio(total.TotalLines, total.Files)),8}");
        builder.Append($"Largest file:         {FormatLargestFile(largestFile)}");

        return builder.ToString();
    }

    private static string FormatJson(
        IReadOnlyList<LanguageCountResult> results,
        LineCountResult total,
        FileCountResult? largestFile)
    {
        object report = new
        {
            languages = results.Select(result => new
            {
                id = result.Language.Id,
                name = result.Language.DisplayName,
                files = result.Result.Files,
                code = result.Result.CodeLines,
                comments = result.Result.CommentLines,
                blank = result.Result.BlankLines,
                total = result.Result.TotalLines,
                metrics = new
                {
                    codeRatio = RoundRatio(result.CodeRatio),
                    commentRatio = RoundRatio(result.CommentRatio),
                    blankRatio = RoundRatio(result.BlankRatio),
                    averageLinesPerFile = RoundNumber(result.AverageLinesPerFile),
                    largestFile = result.LargestFile is null
                        ? null
                        : new
                        {
                            path = result.LargestFile.FilePath,
                            total = result.LargestFile.Result.TotalLines,
                            code = result.LargestFile.Result.CodeLines,
                            comments = result.LargestFile.Result.CommentLines,
                            blank = result.LargestFile.Result.BlankLines
                        }
                }
            }),
            total = new
            {
                files = total.Files,
                code = total.CodeLines,
                comments = total.CommentLines,
                blank = total.BlankLines,
                total = total.TotalLines,
                metrics = new
                {
                    codeRatio = RoundRatio(CalculateRatio(total.CodeLines, total.TotalLines)),
                    commentRatio = RoundRatio(CalculateRatio(total.CommentLines, total.TotalLines)),
                    blankRatio = RoundRatio(CalculateRatio(total.BlankLines, total.TotalLines)),
                    averageLinesPerFile = RoundNumber(CalculateRatio(total.TotalLines, total.Files)),
                    largestFile = largestFile is null
                        ? null
                        : new
                        {
                            path = largestFile.FilePath,
                            total = largestFile.Result.TotalLines,
                            code = largestFile.Result.CodeLines,
                            comments = largestFile.Result.CommentLines,
                            blank = largestFile.Result.BlankLines
                        }
                }
            }
        };

        return JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
    }

    private static string FormatMarkdown(
        IReadOnlyList<LanguageCountResult> results,
        LineCountResult total,
        FileCountResult? largestFile)
    {
        StringBuilder builder = new();
        builder.AppendLine("| Language | Files | Code | Comments | Blank | Total |");
        builder.AppendLine("|---|---:|---:|---:|---:|---:|");

        foreach (LanguageCountResult result in results)
        {
            builder.AppendLine(
                $"| {EscapeMarkdownCell(result.Language.DisplayName)} | {result.Result.Files} | {result.Result.CodeLines} | {result.Result.CommentLines} | {result.Result.BlankLines} | {result.Result.TotalLines} |");
        }

        builder.AppendLine(
            $"| **Total** | **{total.Files}** | **{total.CodeLines}** | **{total.CommentLines}** | **{total.BlankLines}** | **{total.TotalLines}** |");
        builder.AppendLine();
        builder.AppendLine("## Metrics");
        builder.AppendLine();
        builder.AppendLine($"- Comment ratio: {FormatPercentage(CalculateRatio(total.CommentLines, total.TotalLines))}");
        builder.AppendLine($"- Blank ratio: {FormatPercentage(CalculateRatio(total.BlankLines, total.TotalLines))}");
        builder.AppendLine($"- Code ratio: {FormatPercentage(CalculateRatio(total.CodeLines, total.TotalLines))}");
        builder.AppendLine($"- Average lines/file: {FormatNumber(CalculateRatio(total.TotalLines, total.Files))}");
        builder.Append($"- Largest file: {EscapeMarkdownCell(FormatLargestFile(largestFile))}");

        return builder.ToString();
    }

    private static string FormatCsv(
        IReadOnlyList<LanguageCountResult> results,
        LineCountResult total,
        FileCountResult? largestFile)
    {
        StringBuilder builder = new();
        builder.AppendLine("Language,Files,Code,Comments,Blank,Total");

        foreach (LanguageCountResult result in results)
        {
            builder.AppendLine(
                $"{EscapeCsvCell(result.Language.DisplayName)},{result.Result.Files},{result.Result.CodeLines},{result.Result.CommentLines},{result.Result.BlankLines},{result.Result.TotalLines}");
        }

        builder.AppendLine(
            $"{EscapeCsvCell("Total")},{total.Files},{total.CodeLines},{total.CommentLines},{total.BlankLines},{total.TotalLines}");
        builder.AppendLine();
        builder.AppendLine("Metric,Value");
        builder.AppendLine($"Comment ratio,{FormatPercentage(CalculateRatio(total.CommentLines, total.TotalLines))}");
        builder.AppendLine($"Blank ratio,{FormatPercentage(CalculateRatio(total.BlankLines, total.TotalLines))}");
        builder.AppendLine($"Code ratio,{FormatPercentage(CalculateRatio(total.CodeLines, total.TotalLines))}");
        builder.AppendLine($"Average lines/file,{FormatNumber(CalculateRatio(total.TotalLines, total.Files))}");
        builder.Append($"Largest file,{EscapeCsvCell(FormatLargestFile(largestFile))}");

        return builder.ToString();
    }

    private static LineCountResult CalculateTotal(IEnumerable<LanguageCountResult> results)
    {
        return results
            .Select(result => result.Result)
            .Aggregate(LineCountResult.Empty, (current, next) => current.Add(next));
    }

    private static FileCountResult? FindLargestFile(IEnumerable<LanguageCountResult> results)
    {
        return results
            .SelectMany(result => result.FileResults)
            .OrderByDescending(file => file.Result.TotalLines)
            .ThenBy(file => file.FilePath, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();
    }

    private static double CalculateRatio(int value, int total)
    {
        return total == 0 ? 0 : (double)value / total;
    }

    private static double RoundRatio(double value)
    {
        return Math.Round(value, 4, MidpointRounding.AwayFromZero);
    }

    private static double RoundNumber(double value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    private static string FormatPercentage(double value)
    {
        return value.ToString("P2", CultureInfo.InvariantCulture);
    }

    private static string FormatNumber(double value)
    {
        return value.ToString("0.##", CultureInfo.InvariantCulture);
    }

    private static string FormatLargestFile(FileCountResult? largestFile)
    {
        return largestFile is null
            ? "n/a"
            : $"{largestFile.FilePath} ({largestFile.Result.TotalLines} lines)";
    }

    private static string EscapeMarkdownCell(string value)
    {
        return value.Replace("|", "\\|", StringComparison.Ordinal);
    }

    private static string EscapeCsvCell(string value)
    {
        if (!value.Contains(',', StringComparison.Ordinal)
            && !value.Contains('"', StringComparison.Ordinal)
            && !value.Contains('\n', StringComparison.Ordinal)
            && !value.Contains('\r', StringComparison.Ordinal))
        {
            return value;
        }

        return "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
    }
}

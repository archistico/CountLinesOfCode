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

        return format switch
        {
            ReportFormat.Table => FormatTable(visibleResults, total),
            ReportFormat.Json => FormatJson(visibleResults, total),
            ReportFormat.Markdown => FormatMarkdown(visibleResults, total),
            ReportFormat.Csv => FormatCsv(visibleResults, total),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported report format.")
        };
    }

    private static string FormatTable(IReadOnlyList<LanguageCountResult> results, LineCountResult total)
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
        builder.Append(
            $"{"Total",-22} {total.Files,5} {total.CodeLines,9} {total.CommentLines,9} {total.BlankLines,9} {total.TotalLines,9}");

        return builder.ToString();
    }

    private static string FormatJson(IReadOnlyList<LanguageCountResult> results, LineCountResult total)
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
                total = result.Result.TotalLines
            }),
            total = new
            {
                files = total.Files,
                code = total.CodeLines,
                comments = total.CommentLines,
                blank = total.BlankLines,
                total = total.TotalLines
            }
        };

        return JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
    }

    private static string FormatMarkdown(IReadOnlyList<LanguageCountResult> results, LineCountResult total)
    {
        StringBuilder builder = new();
        builder.AppendLine("| Language | Files | Code | Comments | Blank | Total |");
        builder.AppendLine("|---|---:|---:|---:|---:|---:|");

        foreach (LanguageCountResult result in results)
        {
            builder.AppendLine(
                $"| {EscapeMarkdownCell(result.Language.DisplayName)} | {result.Result.Files} | {result.Result.CodeLines} | {result.Result.CommentLines} | {result.Result.BlankLines} | {result.Result.TotalLines} |");
        }

        builder.Append(
            $"| **Total** | **{total.Files}** | **{total.CodeLines}** | **{total.CommentLines}** | **{total.BlankLines}** | **{total.TotalLines}** |");

        return builder.ToString();
    }

    private static string FormatCsv(IReadOnlyList<LanguageCountResult> results, LineCountResult total)
    {
        StringBuilder builder = new();
        builder.AppendLine("Language,Files,Code,Comments,Blank,Total");

        foreach (LanguageCountResult result in results)
        {
            builder.AppendLine(
                $"{EscapeCsvCell(result.Language.DisplayName)},{result.Result.Files},{result.Result.CodeLines},{result.Result.CommentLines},{result.Result.BlankLines},{result.Result.TotalLines}");
        }

        builder.Append(
            $"{EscapeCsvCell("Total")},{total.Files},{total.CodeLines},{total.CommentLines},{total.BlankLines},{total.TotalLines}");

        return builder.ToString();
    }

    private static LineCountResult CalculateTotal(IEnumerable<LanguageCountResult> results)
    {
        return results
            .Select(result => result.Result)
            .Aggregate(LineCountResult.Empty, (current, next) => current.Add(next));
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

namespace clsoc;

public static class ReportFormatParser
{
    public static bool TryParse(string value, out ReportFormat format)
    {
        format = ReportFormat.Table;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "table" => Set(ReportFormat.Table, out format),
            "json" => Set(ReportFormat.Json, out format),
            "markdown" => Set(ReportFormat.Markdown, out format),
            "md" => Set(ReportFormat.Markdown, out format),
            "csv" => Set(ReportFormat.Csv, out format),
            _ => false
        };
    }

    private static bool Set(ReportFormat value, out ReportFormat format)
    {
        format = value;
        return true;
    }
}

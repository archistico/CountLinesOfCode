namespace clsoc;

public sealed class LanguageCountResult
{
    public LanguageCountResult(LanguageDefinition language, LineCountResult result)
        : this(language, result, Array.Empty<FileCountResult>())
    {
    }

    public LanguageCountResult(
        LanguageDefinition language,
        LineCountResult result,
        IReadOnlyList<FileCountResult> fileResults)
    {
        this.Language = language ?? throw new ArgumentNullException(nameof(language));
        this.Result = result ?? throw new ArgumentNullException(nameof(result));
        this.FileResults = fileResults ?? throw new ArgumentNullException(nameof(fileResults));
    }

    public LanguageDefinition Language { get; }

    public LineCountResult Result { get; }

    public IReadOnlyList<FileCountResult> FileResults { get; }

    public FileCountResult? LargestFile => this.FileResults
        .OrderByDescending(file => file.Result.TotalLines)
        .ThenBy(file => file.FilePath, StringComparer.OrdinalIgnoreCase)
        .FirstOrDefault();

    public double AverageLinesPerFile => CalculateRatio(this.Result.TotalLines, this.Result.Files);

    public double CodeRatio => CalculateRatio(this.Result.CodeLines, this.Result.TotalLines);

    public double CommentRatio => CalculateRatio(this.Result.CommentLines, this.Result.TotalLines);

    public double BlankRatio => CalculateRatio(this.Result.BlankLines, this.Result.TotalLines);

    private static double CalculateRatio(int value, int total)
    {
        return total == 0 ? 0 : (double)value / total;
    }
}

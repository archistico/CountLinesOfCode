namespace clsoc;

public sealed class ProjectCounter
{
    private readonly FileScanner fileScanner;
    private readonly LineCounter lineCounter;

    public ProjectCounter()
        : this(new FileScanner(), new LineCounter())
    {
    }

    public ProjectCounter(FileScanner fileScanner, LineCounter lineCounter)
    {
        this.fileScanner = fileScanner ?? throw new ArgumentNullException(nameof(fileScanner));
        this.lineCounter = lineCounter ?? throw new ArgumentNullException(nameof(lineCounter));
    }

    public IReadOnlyList<LanguageCountResult> CountByLanguage(string rootPath, IEnumerable<LanguageDefinition> languages)
    {
        return this.CountByLanguage(rootPath, languages, FileScanOptions.Default);
    }

    public IReadOnlyList<LanguageCountResult> CountByLanguage(
        string rootPath,
        IEnumerable<LanguageDefinition> languages,
        FileScanOptions scanOptions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(scanOptions);

        List<LanguageCountResult> results = new();

        foreach (LanguageDefinition language in languages)
        {
            IReadOnlyList<string> files = this.fileScanner.FindFiles(rootPath, language.Extensions, scanOptions);
            LineCountResult summary = LineCountResult.Empty;
            List<FileCountResult> fileResults = new();

            foreach (string file in files)
            {
                LineCountResult fileResult = this.lineCounter.CountFile(file, language);
                summary = summary.Add(fileResult);
                fileResults.Add(new FileCountResult(file, fileResult));
            }

            results.Add(new LanguageCountResult(language, summary, fileResults));
        }

        return results;
    }

    public LineCountResult CountTotal(string rootPath, IEnumerable<LanguageDefinition> languages)
    {
        return this.CountTotal(rootPath, languages, FileScanOptions.Default);
    }

    public LineCountResult CountTotal(string rootPath, IEnumerable<LanguageDefinition> languages, FileScanOptions scanOptions)
    {
        return this.CountByLanguage(rootPath, languages, scanOptions)
            .Select(result => result.Result)
            .Aggregate(LineCountResult.Empty, (current, next) => current.Add(next));
    }
}

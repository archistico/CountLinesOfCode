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
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);
        ArgumentNullException.ThrowIfNull(languages);

        List<LanguageCountResult> results = new();

        foreach (LanguageDefinition language in languages)
        {
            IReadOnlyList<string> files = this.fileScanner.FindFiles(rootPath, language.Extensions);
            LineCountResult summary = LineCountResult.Empty;

            foreach (string file in files)
            {
                summary = summary.Add(this.lineCounter.CountFile(file, language));
            }

            results.Add(new LanguageCountResult(language, summary));
        }

        return results;
    }

    public LineCountResult CountTotal(string rootPath, IEnumerable<LanguageDefinition> languages)
    {
        return this.CountByLanguage(rootPath, languages)
            .Select(result => result.Result)
            .Aggregate(LineCountResult.Empty, (current, next) => current.Add(next));
    }
}

namespace clsoc;

public sealed class LanguageCountResult
{
    public LanguageCountResult(LanguageDefinition language, LineCountResult result)
    {
        this.Language = language ?? throw new ArgumentNullException(nameof(language));
        this.Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    public LanguageDefinition Language { get; }

    public LineCountResult Result { get; }
}

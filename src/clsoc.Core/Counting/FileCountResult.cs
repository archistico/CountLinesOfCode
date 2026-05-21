namespace clsoc;

public sealed class FileCountResult
{
    public FileCountResult(string filePath, LineCountResult result)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(result);

        this.FilePath = filePath;
        this.Result = result;
    }

    public string FilePath { get; }

    public LineCountResult Result { get; }
}

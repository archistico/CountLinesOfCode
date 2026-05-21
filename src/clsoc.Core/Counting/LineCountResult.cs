namespace clsoc;

public sealed class LineCountResult
{
    public LineCountResult(int files, int totalLines, int codeLines, int blankLines, int commentLines)
    {
        this.Files = files;
        this.TotalLines = totalLines;
        this.CodeLines = codeLines;
        this.BlankLines = blankLines;
        this.CommentLines = commentLines;
    }

    public int Files { get; }

    public int TotalLines { get; }

    public int CodeLines { get; }

    public int BlankLines { get; }

    public int CommentLines { get; }

    public static LineCountResult Empty { get; } = new(0, 0, 0, 0, 0);

    public LineCountResult Add(LineCountResult other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return new LineCountResult(
            this.Files + other.Files,
            this.TotalLines + other.TotalLines,
            this.CodeLines + other.CodeLines,
            this.BlankLines + other.BlankLines,
            this.CommentLines + other.CommentLines);
    }
}

namespace clsoc;

public sealed class CommentBlockDefinition
{
    public CommentBlockDefinition(string startToken, string endToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(startToken);
        ArgumentException.ThrowIfNullOrWhiteSpace(endToken);

        this.StartToken = startToken;
        this.EndToken = endToken;
    }

    public string StartToken { get; }

    public string EndToken { get; }
}

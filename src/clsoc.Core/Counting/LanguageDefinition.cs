namespace clsoc;

public sealed class LanguageDefinition
{
    public LanguageDefinition(
        string id,
        string displayName,
        IEnumerable<string> extensions,
        IEnumerable<string> lineCommentTokens,
        IEnumerable<CommentBlockDefinition> blockComments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(extensions);
        ArgumentNullException.ThrowIfNull(lineCommentTokens);
        ArgumentNullException.ThrowIfNull(blockComments);

        this.Id = id.Trim();
        this.DisplayName = displayName.Trim();
        this.Extensions = extensions
            .Select(NormalizeExtension)
            .Where(extension => extension.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        this.LineCommentTokens = lineCommentTokens
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .Distinct(StringComparer.Ordinal)
            .OrderByDescending(token => token.Length)
            .ToArray();
        this.BlockComments = blockComments.ToArray();

        if (this.Extensions.Count == 0)
        {
            throw new ArgumentException("At least one extension is required.", nameof(extensions));
        }
    }

    public string Id { get; }

    public string DisplayName { get; }

    public IReadOnlyList<string> Extensions { get; }

    public IReadOnlyList<string> LineCommentTokens { get; }

    public IReadOnlyList<CommentBlockDefinition> BlockComments { get; }

    public bool MatchesExtension(string extension)
    {
        string normalizedExtension = NormalizeExtension(extension);
        return this.Extensions.Contains(normalizedExtension, StringComparer.OrdinalIgnoreCase);
    }

    public static string NormalizeExtension(string extension)
    {
        return extension.Trim().TrimStart('.');
    }
}

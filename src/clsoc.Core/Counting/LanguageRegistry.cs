namespace clsoc;

public static class LanguageRegistry
{
    public static LanguageDefinition Legacy { get; } = new(
        "legacy",
        "Legacy",
        new[] { "cs" },
        new[] { "//", "'" },
        new[] { new CommentBlockDefinition("/*", "*/") });

    public static LanguageDefinition CSharp { get; } = new(
        "csharp",
        "C#",
        new[] { "cs" },
        new[] { "//" },
        new[] { new CommentBlockDefinition("/*", "*/") });

    public static LanguageDefinition VisualBasic { get; } = new(
        "vb",
        "VB.NET",
        new[] { "vb" },
        new[] { "'" },
        Array.Empty<CommentBlockDefinition>());

    public static LanguageDefinition JavaScript { get; } = new(
        "javascript",
        "JavaScript/TypeScript",
        new[] { "js", "jsx", "ts", "tsx" },
        new[] { "//" },
        new[] { new CommentBlockDefinition("/*", "*/") });

    public static LanguageDefinition Css { get; } = new(
        "css",
        "CSS",
        new[] { "css", "scss", "sass", "less" },
        Array.Empty<string>(),
        new[] { new CommentBlockDefinition("/*", "*/") });

    public static LanguageDefinition Python { get; } = new(
        "python",
        "Python",
        new[] { "py" },
        new[] { "#" },
        Array.Empty<CommentBlockDefinition>());

    public static LanguageDefinition Sql { get; } = new(
        "sql",
        "SQL",
        new[] { "sql" },
        new[] { "--" },
        new[] { new CommentBlockDefinition("/*", "*/") });

    public static LanguageDefinition Xml { get; } = new(
        "xml",
        "XML/XAML",
        new[] { "xml", "xaml", "csproj", "props", "targets", "config" },
        Array.Empty<string>(),
        new[] { new CommentBlockDefinition("<!--", "-->") });

    public static LanguageDefinition Html { get; } = new(
        "html",
        "HTML",
        new[] { "html", "htm" },
        Array.Empty<string>(),
        new[] { new CommentBlockDefinition("<!--", "-->") });

    public static IReadOnlyList<LanguageDefinition> All { get; } = new[]
    {
        CSharp,
        VisualBasic,
        JavaScript,
        Css,
        Python,
        Sql,
        Xml,
        Html,
    };

    public static LanguageDefinition? FindById(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return All.FirstOrDefault(language => string.Equals(language.Id, id.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static IReadOnlyList<LanguageDefinition> FindByExtensions(IEnumerable<string> extensions)
    {
        ArgumentNullException.ThrowIfNull(extensions);

        string[] normalizedExtensions = extensions
            .Select(LanguageDefinition.NormalizeExtension)
            .Where(extension => extension.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return All
            .Where(language => normalizedExtensions.Any(language.MatchesExtension))
            .ToArray();
    }
}

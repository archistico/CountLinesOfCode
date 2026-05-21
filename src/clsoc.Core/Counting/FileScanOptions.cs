namespace clsoc;

public sealed class FileScanOptions
{
    private static readonly string[] DefaultExcludedDirectoryNames =
    {
        ".git",
        ".vs",
        "bin",
        "obj",
        "node_modules",
        "dist",
        "build",
        "packages",
        "vendor",
        "coverage"
    };

    public FileScanOptions()
        : this(useDefaultExcludes: true, excludedDirectoryNames: Array.Empty<string>())
    {
    }

    public FileScanOptions(bool useDefaultExcludes, IEnumerable<string> excludedDirectoryNames)
    {
        ArgumentNullException.ThrowIfNull(excludedDirectoryNames);

        this.UseDefaultExcludes = useDefaultExcludes;
        this.ExcludedDirectoryNames = excludedDirectoryNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(NormalizeDirectoryName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public bool UseDefaultExcludes { get; }

    public IReadOnlyList<string> ExcludedDirectoryNames { get; }

    public static FileScanOptions Default { get; } = new();

    public IReadOnlySet<string> CreateExcludedDirectorySet()
    {
        HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);

        if (this.UseDefaultExcludes)
        {
            foreach (string name in DefaultExcludedDirectoryNames)
            {
                names.Add(NormalizeDirectoryName(name));
            }
        }

        foreach (string name in this.ExcludedDirectoryNames)
        {
            names.Add(NormalizeDirectoryName(name));
        }

        return names;
    }

    private static string NormalizeDirectoryName(string name)
    {
        return name.Trim().Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}

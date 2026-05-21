namespace clsoc;

public sealed class FileScanner
{
    public IReadOnlyList<string> FindFiles(string rootPath, string extension)
    {
        return this.FindFiles(rootPath, new[] { extension }, FileScanOptions.Default);
    }

    public IReadOnlyList<string> FindFiles(string rootPath, string extension, FileScanOptions options)
    {
        return this.FindFiles(rootPath, new[] { extension }, options);
    }

    public IReadOnlyList<string> FindFiles(string rootPath, IEnumerable<string> extensions)
    {
        return this.FindFiles(rootPath, extensions, FileScanOptions.Default);
    }

    public IReadOnlyList<string> FindFiles(string rootPath, IEnumerable<string> extensions, FileScanOptions options)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("Il percorso da analizzare non può essere vuoto.", nameof(rootPath));
        }

        ArgumentNullException.ThrowIfNull(extensions);
        ArgumentNullException.ThrowIfNull(options);

        string[] normalizedExtensions = extensions
            .Select(LanguageDefinition.NormalizeExtension)
            .Where(extension => extension.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalizedExtensions.Length == 0)
        {
            throw new ArgumentException("Almeno un'estensione da analizzare è obbligatoria.", nameof(extensions));
        }

        if (!Directory.Exists(rootPath))
        {
            throw new DirectoryNotFoundException($"La directory '{rootPath}' non esiste.");
        }

        HashSet<string> extensionSet = new(normalizedExtensions, StringComparer.OrdinalIgnoreCase);
        IReadOnlySet<string> excludedDirectoryNames = options.CreateExcludedDirectorySet();

        return EnumerateFiles(rootPath, extensionSet, excludedDirectoryNames)
            .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IEnumerable<string> EnumerateFiles(
        string directory,
        IReadOnlySet<string> extensionSet,
        IReadOnlySet<string> excludedDirectoryNames)
    {
        foreach (string file in Directory.EnumerateFiles(directory))
        {
            if (extensionSet.Contains(LanguageDefinition.NormalizeExtension(Path.GetExtension(file))))
            {
                yield return file;
            }
        }

        foreach (string childDirectory in Directory.EnumerateDirectories(directory))
        {
            string directoryName = Path.GetFileName(childDirectory);
            if (excludedDirectoryNames.Contains(directoryName))
            {
                continue;
            }

            foreach (string file in EnumerateFiles(childDirectory, extensionSet, excludedDirectoryNames))
            {
                yield return file;
            }
        }
    }
}

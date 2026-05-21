namespace clsoc;

public sealed class FileScanner
{
    public IReadOnlyList<string> FindFiles(string rootPath, string extension)
    {
        return this.FindFiles(rootPath, new[] { extension });
    }

    public IReadOnlyList<string> FindFiles(string rootPath, IEnumerable<string> extensions)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("Il percorso da analizzare non può essere vuoto.", nameof(rootPath));
        }

        ArgumentNullException.ThrowIfNull(extensions);

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

        return Directory
            .EnumerateFiles(rootPath, "*", SearchOption.AllDirectories)
            .Where(file => extensionSet.Contains(LanguageDefinition.NormalizeExtension(Path.GetExtension(file))))
            .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}

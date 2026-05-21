namespace clsoc;

public sealed class FileScanner
{
    public IReadOnlyList<string> FindFiles(string rootPath, string extension)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("Il percorso da analizzare non può essere vuoto.", nameof(rootPath));
        }

        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ArgumentException("L'estensione da analizzare non può essere vuota.", nameof(extension));
        }

        if (!Directory.Exists(rootPath))
        {
            throw new DirectoryNotFoundException($"La directory '{rootPath}' non esiste.");
        }

        string normalizedExtension = NormalizeExtension(extension);
        return Directory
            .GetFiles(rootPath, "*." + normalizedExtension, SearchOption.AllDirectories)
            .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string NormalizeExtension(string extension)
    {
        return extension.Trim().TrimStart('.');
    }
}

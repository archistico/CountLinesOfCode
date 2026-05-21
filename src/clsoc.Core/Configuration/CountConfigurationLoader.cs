using System.Text.Json;

namespace clsoc;

public sealed class CountConfigurationLoader
{
    public const string DefaultFileName = "clsoc.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public CountConfiguration? LoadIfExists(string rootPath, string? explicitConfigPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);

        string? configPath = ResolveConfigPath(rootPath, explicitConfigPath);
        if (configPath is null || !File.Exists(configPath))
        {
            return null;
        }

        return this.Load(configPath);
    }

    public CountConfiguration Load(string configPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(configPath);

        using FileStream stream = File.OpenRead(configPath);
        CountConfiguration? configuration = JsonSerializer.Deserialize<CountConfiguration>(stream, SerializerOptions);

        return configuration ?? new CountConfiguration();
    }

    public static string? ResolveConfigPath(string rootPath, string? explicitConfigPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);

        if (!string.IsNullOrWhiteSpace(explicitConfigPath))
        {
            return Path.GetFullPath(explicitConfigPath);
        }

        string candidate = Path.Combine(Path.GetFullPath(rootPath), DefaultFileName);
        return File.Exists(candidate) ? candidate : null;
    }
}

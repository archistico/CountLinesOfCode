namespace clsoc;

public sealed class CountConfiguration
{
    public string? RootPath { get; init; }

    public string[]? Languages { get; init; }

    public string[]? Extensions { get; init; }

    public string[]? ExcludeDirectories { get; init; }

    public bool? UseDefaultExcludes { get; init; }

    public string? OutputFormat { get; init; }

    public string? OutputPath { get; init; }
}

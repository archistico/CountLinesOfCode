namespace clsoc;

internal static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return 1;
        }

        if (!string.Equals(args[0], "count", StringComparison.OrdinalIgnoreCase))
        {
            Contatore contatore = new();
            contatore.Conteggia(args[0]);
            return 0;
        }

        CountOptions options = CountOptions.Parse(args.Skip(1).ToArray());
        if (options.ShowHelp)
        {
            PrintUsage();
            return 0;
        }

        IReadOnlyList<LanguageDefinition> languages = ResolveLanguages(options);
        if (languages.Count == 0)
        {
            Console.WriteLine("Nessun linguaggio trovato per i filtri indicati.");
            return 1;
        }

        ProjectCounter counter = new();
        IReadOnlyList<LanguageCountResult> results = counter.CountByLanguage(options.RootPath, languages, options.ScanOptions);

        CountReportFormatter formatter = new();
        string report = formatter.Format(results, options.OutputFormat);

        if (!string.IsNullOrWhiteSpace(options.OutputPath))
        {
            string? outputDirectory = Path.GetDirectoryName(Path.GetFullPath(options.OutputPath));
            if (!string.IsNullOrWhiteSpace(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            File.WriteAllText(options.OutputPath, report);
        }
        else
        {
            Console.WriteLine(report);
        }

        return 0;
    }

    private static IReadOnlyList<LanguageDefinition> ResolveLanguages(CountOptions options)
    {
        if (options.LanguageIds.Count > 0)
        {
            List<LanguageDefinition> languages = new();

            foreach (string languageId in options.LanguageIds)
            {
                LanguageDefinition? language = LanguageRegistry.FindById(languageId);
                if (language is not null)
                {
                    languages.Add(language);
                }
            }

            return languages;
        }

        if (options.Extensions.Count > 0)
        {
            return LanguageRegistry.FindByExtensions(options.Extensions);
        }

        return LanguageRegistry.All;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Uso legacy:");
        Console.WriteLine("  clsoc cs");
        Console.WriteLine();
        Console.WriteLine("Uso nuovo:");
        Console.WriteLine("  clsoc count .");
        Console.WriteLine("  clsoc count . --lang csharp");
        Console.WriteLine("  clsoc count . --lang csharp,xml");
        Console.WriteLine("  clsoc count . --ext cs,xaml,xml");
        Console.WriteLine("  clsoc count . --exclude bin,obj");
        Console.WriteLine("  clsoc count . --no-default-excludes");
        Console.WriteLine("  clsoc count . --format table");
        Console.WriteLine("  clsoc count . --format json");
        Console.WriteLine("  clsoc count . --format markdown");
        Console.WriteLine("  clsoc count . --format csv");
        Console.WriteLine("  clsoc count . --format json --output report.json");
    }

    private sealed class CountOptions
    {
        private CountOptions(
            string rootPath,
            IReadOnlyList<string> languageIds,
            IReadOnlyList<string> extensions,
            FileScanOptions scanOptions,
            ReportFormat outputFormat,
            string? outputPath,
            bool showHelp)
        {
            this.RootPath = rootPath;
            this.LanguageIds = languageIds;
            this.Extensions = extensions;
            this.ScanOptions = scanOptions;
            this.OutputFormat = outputFormat;
            this.OutputPath = outputPath;
            this.ShowHelp = showHelp;
        }

        public string RootPath { get; }

        public IReadOnlyList<string> LanguageIds { get; }

        public IReadOnlyList<string> Extensions { get; }

        public FileScanOptions ScanOptions { get; }

        public ReportFormat OutputFormat { get; }

        public string? OutputPath { get; }

        public bool ShowHelp { get; }

        public static CountOptions Parse(string[] args)
        {
            string rootPath = Environment.CurrentDirectory;
            List<string> languageIds = new();
            List<string> extensions = new();
            bool showHelp = false;
            bool useDefaultExcludes = true;
            List<string> excludedDirectoryNames = new();
            ReportFormat outputFormat = ReportFormat.Table;
            string? outputPath = null;

            int index = 0;
            if (index < args.Length && !args[index].StartsWith("--", StringComparison.Ordinal))
            {
                rootPath = args[index];
                index++;
            }

            while (index < args.Length)
            {
                string option = args[index];
                string? value = index + 1 < args.Length ? args[index + 1] : null;

                if (string.Equals(option, "--help", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(option, "-h", StringComparison.OrdinalIgnoreCase))
                {
                    showHelp = true;
                    index++;
                    continue;
                }

                if (string.Equals(option, "--lang", StringComparison.OrdinalIgnoreCase) && value is not null)
                {
                    languageIds.AddRange(SplitList(value));
                    index += 2;
                    continue;
                }

                if (string.Equals(option, "--ext", StringComparison.OrdinalIgnoreCase) && value is not null)
                {
                    extensions.AddRange(SplitList(value));
                    index += 2;
                    continue;
                }

                if (string.Equals(option, "--exclude", StringComparison.OrdinalIgnoreCase) && value is not null)
                {
                    excludedDirectoryNames.AddRange(SplitList(value));
                    index += 2;
                    continue;
                }

                if (string.Equals(option, "--no-default-excludes", StringComparison.OrdinalIgnoreCase))
                {
                    useDefaultExcludes = false;
                    index++;
                    continue;
                }

                if (string.Equals(option, "--format", StringComparison.OrdinalIgnoreCase) && value is not null)
                {
                    if (ReportFormatParser.TryParse(value, out ReportFormat parsedFormat))
                    {
                        outputFormat = parsedFormat;
                    }

                    index += 2;
                    continue;
                }

                if (string.Equals(option, "--output", StringComparison.OrdinalIgnoreCase) && value is not null)
                {
                    outputPath = value;
                    index += 2;
                    continue;
                }

                index++;
            }

            return new CountOptions(
                rootPath,
                languageIds,
                extensions,
                new FileScanOptions(useDefaultExcludes, excludedDirectoryNames),
                outputFormat,
                outputPath,
                showHelp);
        }

        private static IEnumerable<string> SplitList(string value)
        {
            return value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(item => item.Length > 0);
        }
    }
}

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
        IReadOnlyList<LanguageCountResult> results = counter.CountByLanguage(options.RootPath, languages);
        PrintResults(results);
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

    private static void PrintResults(IReadOnlyList<LanguageCountResult> results)
    {
        Console.WriteLine("Language                 Files      Code  Comments     Blank     Total");
        Console.WriteLine("-----------------------------------------------------------------------");

        LineCountResult total = LineCountResult.Empty;

        foreach (LanguageCountResult result in results.Where(result => result.Result.Files > 0))
        {
            total = total.Add(result.Result);
            Console.WriteLine(
                $"{result.Language.DisplayName,-22} {result.Result.Files,5} {result.Result.CodeLines,9} {result.Result.CommentLines,9} {result.Result.BlankLines,9} {result.Result.TotalLines,9}");
        }

        Console.WriteLine("-----------------------------------------------------------------------");
        Console.WriteLine(
            $"{"Total",-22} {total.Files,5} {total.CodeLines,9} {total.CommentLines,9} {total.BlankLines,9} {total.TotalLines,9}");
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
    }

    private sealed class CountOptions
    {
        private CountOptions(string rootPath, IReadOnlyList<string> languageIds, IReadOnlyList<string> extensions, bool showHelp)
        {
            this.RootPath = rootPath;
            this.LanguageIds = languageIds;
            this.Extensions = extensions;
            this.ShowHelp = showHelp;
        }

        public string RootPath { get; }

        public IReadOnlyList<string> LanguageIds { get; }

        public IReadOnlyList<string> Extensions { get; }

        public bool ShowHelp { get; }

        public static CountOptions Parse(string[] args)
        {
            string rootPath = Environment.CurrentDirectory;
            List<string> languageIds = new();
            List<string> extensions = new();
            bool showHelp = false;

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

                index++;
            }

            return new CountOptions(rootPath, languageIds, extensions, showHelp);
        }

        private static IEnumerable<string> SplitList(string value)
        {
            return value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(item => item.Length > 0);
        }
    }
}

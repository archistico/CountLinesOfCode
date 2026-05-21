namespace clsoc;

public class Contatore
{
    private readonly FileScanner fileScanner;
    private readonly LineCounter lineCounter;

    public int linesOfCode;
    public int linesOfcodeEmpty;
    public int linesOfcodeComment;
    public int linesOfcodeTotal;
    public int numberOfFile;

    public Contatore()
        : this(new FileScanner(), new LineCounter())
    {
    }

    public Contatore(FileScanner fileScanner, LineCounter lineCounter)
    {
        this.fileScanner = fileScanner ?? throw new ArgumentNullException(nameof(fileScanner));
        this.lineCounter = lineCounter ?? throw new ArgumentNullException(nameof(lineCounter));
        this.Reset();
    }

    public void Conteggia(string ext)
    {
        Console.WriteLine("CALCOLO NUMERO DI LINEE");
        this.ConteggiaLinee(ext);
        this.MostraRisultati();
    }

    public void ConteggiaLinee(string ext)
    {
        this.ConteggiaLinee(ext, Environment.CurrentDirectory);
    }

    public void ConteggiaLinee(string ext, string rootPath)
    {
        this.Reset();

        IReadOnlyList<string> files = this.fileScanner.FindFiles(rootPath, ext);
        LineCountResult summary = LineCountResult.Empty;

        foreach (string file in files)
        {
            summary = summary.Add(this.lineCounter.CountFile(file));
        }

        this.Apply(summary, files.Count);
    }

    public void ConteggiaLinguaggio(LanguageDefinition language, string rootPath)
    {
        ArgumentNullException.ThrowIfNull(language);
        this.Reset();

        IReadOnlyList<string> files = this.fileScanner.FindFiles(rootPath, language.Extensions);
        LineCountResult summary = LineCountResult.Empty;

        foreach (string file in files)
        {
            summary = summary.Add(this.lineCounter.CountFile(file, language));
        }

        this.Apply(summary, files.Count);
    }

    public void MostraRisultati()
    {
        Console.WriteLine("Numero file     : " + this.numberOfFile.ToString());
        Console.WriteLine("Linee totali    : " + this.linesOfcodeTotal.ToString());
        Console.WriteLine("-----------------------");
        Console.WriteLine("Linee di codice : " + this.linesOfCode.ToString());
        Console.WriteLine("Linee vuote     : " + this.linesOfcodeEmpty.ToString());
        Console.WriteLine("Linee commenti  : " + this.linesOfcodeComment.ToString());
    }

    private void Apply(LineCountResult summary, int fileCount)
    {
        this.numberOfFile = fileCount;
        this.linesOfcodeTotal = summary.TotalLines;
        this.linesOfCode = summary.CodeLines;
        this.linesOfcodeEmpty = summary.BlankLines;
        this.linesOfcodeComment = summary.CommentLines;
    }

    private void Reset()
    {
        this.linesOfCode = 0;
        this.linesOfcodeEmpty = 0;
        this.linesOfcodeComment = 0;
        this.linesOfcodeTotal = 0;
        this.numberOfFile = 0;
    }
}

namespace clsoc;

public class Contatore
{
    public int linesOfCode;
    public int linesOfcodeEmpty;
    public int linesOfcodeComment;
    public int linesOfcodeTotal;
    public int numberOfFile;

    public Contatore()
    {
        this.linesOfCode = 0;
        this.linesOfcodeEmpty = 0;
        this.linesOfcodeComment = 0;
        this.linesOfcodeTotal = 0;
        this.numberOfFile = 0;
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
        if (string.IsNullOrWhiteSpace(ext))
        {
            throw new ArgumentException("L'estensione da analizzare non può essere vuota.", nameof(ext));
        }

        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("Il percorso da analizzare non può essere vuoto.", nameof(rootPath));
        }

        if (!Directory.Exists(rootPath))
        {
            throw new DirectoryNotFoundException($"La directory '{rootPath}' non esiste.");
        }

        string normalizedExtension = ext.Trim().TrimStart('.');
        string[] files = Directory.GetFiles(rootPath, "*." + normalizedExtension, SearchOption.AllDirectories);
        this.numberOfFile = files.Length;

        foreach (var file in files)
        {
            this.ConteggiaFile(file);
        }
    }

    private void ConteggiaFile(string file)
    {
        string[] lines = File.ReadAllLines(file);
        this.linesOfcodeTotal += lines.Length;
        bool commento = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            line = line.Trim();

            if (line == string.Empty)
            {
                this.linesOfcodeEmpty++;
            }
            else
            {
                if (commento == false && !line.StartsWith("/*") && !line.StartsWith("//") && !line.StartsWith("'"))
                {
                    this.linesOfCode++;
                }
                else if (commento == false && (line.StartsWith("//") || line.StartsWith("'")))
                {
                    commento = false;
                    this.linesOfcodeComment++;
                }
                else
                {
                    if (commento == true && !line.EndsWith("*/"))
                    {
                        this.linesOfcodeComment++;
                    }
                    else if (commento == true && line.EndsWith("*/"))
                    {
                        commento = false;
                        this.linesOfcodeComment++;
                    }
                    else if (commento == false && line.StartsWith("/*"))
                    {
                        commento = true;
                        this.linesOfcodeComment++;
                    }
                }
            }
        }
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
}

namespace clsoc;

internal static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Inserire l'estensione dei file da analizzare");
            Console.WriteLine("Esempio: clsoc.exe cs");
            return 1;
        }

        Contatore contatore = new();
        contatore.Conteggia(args[0]);
        return 0;
    }
}

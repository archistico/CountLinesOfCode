using System;

namespace clsoc
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				System.Console.WriteLine("Inserire l'estensione dei file da analizzare");
				System.Console.WriteLine("Esempio: ./clsoc.exe txt");
				Environment.Exit(1);
			}

			Contatore cont = new Contatore();
			cont.Conteggia((string)args[0]);
		}
	}
}

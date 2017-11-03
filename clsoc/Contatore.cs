	using System;
	using System.IO;

	namespace clsoc
	{
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
				// PHP, JS, C#
				this.ConteggiaLinee(ext);
				this.MostraRisultati();
			}

			public void ConteggiaLinee(string ext)
			{
				string[] filesTxt = Directory.GetFiles(Environment.CurrentDirectory, "*."+ext, SearchOption.AllDirectories);
				this.numberOfFile = filesTxt.Length;

				foreach (var file in filesTxt)
				{
					string[] lines = System.IO.File.ReadAllLines(file);
					this.linesOfcodeTotal += lines.Length;
					bool commento = false;

					for (int i = 0; i < lines.Length; i++)
					{
						string line = lines[i];
						line = line.Trim();

						if (line == string.Empty)
						{
							this.linesOfcodeEmpty++;
							//Console.WriteLine(line + " -> Linea vuota");
						}
						else {
						// Verifica se la linea è vuota o è un commento
						if (commento == false && !line.StartsWith("/*") && !line.StartsWith("//"))
						{
							// Se non è all'interno di un commento e non inizia per ..
							this.linesOfCode++;
							//Console.WriteLine(line + " -> Linea codice");
						}
						else if (commento == false && line.StartsWith("//"))
						{
							// E' un commento singolo
							commento = false;
							this.linesOfcodeComment++;
							//Console.WriteLine(line + " -> Commento singolo");
						}
						else {
							// Tre casi ora
							// 1 continua un commento
							// 2 finisce un commento
							// 3 inizia un commento
							if (commento == true && !line.EndsWith("*/"))
							{
								this.linesOfcodeComment++;
								//Console.WriteLine(line + " -> Commento continua");
							}
							else if (commento == true && line.EndsWith("*/"))
							{
								commento = false;
								this.linesOfcodeComment++;
								//Console.WriteLine(line + " -> Commento finisce");
							}
							else if (commento == false && line.StartsWith("/*")) {
								commento = true;
								this.linesOfcodeComment++;
								//Console.WriteLine(line + " -> Commento inizia");
							}

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
	}

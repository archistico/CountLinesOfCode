using System;
using System.IO;

namespace clsoc
{
	public class Contatore
	{
		public int lTxt;
		public int lTxtEmpty;
		public int lTxtComment;
		public int nbTxt;

		public Contatore()
		{
			this.lTxt = 0;
			this.lTxtEmpty = 0;
			this.lTxtComment = 0;
			this.nbTxt = 0;
		}

		public void Conteggia()
		{
			Console.WriteLine("CALCOLO NUMERO DI LINEE");
			// HTML, PHP, JS, C#
			this.ConteggiaPhp();
			this.MostraRisultati();
		}

		public void ConteggiaPhp()
		{
			string[] filesTxt = Directory.GetFiles(Environment.CurrentDirectory, "*.txt", SearchOption.AllDirectories);
			this.nbTxt = filesTxt.Length;

			foreach (var file in filesTxt)
			{
				string[] lines = System.IO.File.ReadAllLines(file);
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					if (line == string.Empty)
					{
						this.lTxtEmpty++;
					}
					else {
						this.lTxt++;
					}
				}
			}
		}

		public void MostraRisultati()
		{
			Console.WriteLine("TXT numero file   : " + this.nbTxt.ToString());
			Console.WriteLine("TXT linee         : " + this.lTxt.ToString());
			Console.WriteLine("TXT linee vuote   : " +this.lTxtEmpty.ToString());
			Console.WriteLine("TXT linee commenti: " + this.lTxtComment.ToString());
		}
}
}

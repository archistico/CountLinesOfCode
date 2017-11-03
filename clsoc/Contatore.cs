using System;
using System.IO;

namespace clsoc
{
	public class Contatore
	{
		public Contatore()
		{
		}

		public void Conteggia()
		{
			// HTML, PHP, JS, C#
			string[] filesTxt = Directory.GetFiles(@"\", "*.txt", SearchOption.AllDirectories);
			foreach (var file in filesTxt)
			{
				Console.WriteLine(file);
			}
		}
	}
}

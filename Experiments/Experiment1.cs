using Library;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class Experiment1
	{
		public static void Do()
		{
			var graphFolder = "Graph//ForTraining";
			var files = Directory.GetFiles(Disk2._programFiles + graphFolder);
			var graphL = new List<float>();

			for (int f = 0; f < 1; f++)
			{
				Logger.Log(files[f]);

				string[] lines = File.ReadAllLines(files[f]);

				for (int l = 0; l < lines.Length; l++)
					graphL.Add(Convert.ToSingle(lines[l], CultureInfo.InvariantCulture));
			}

			Do2(1);
			Do2(5);
			Do2(15);

			float[] ar1 = new float[10000];
			for (int i = 1; i < 10000; i++)
				ar1[i] = ar1[i - 1] + Math2.rnd.NextSingle() - 0.5f;

			string csv = "";

			for (int i = 0; i < ar1.Length; i++)
				csv += $"{ar1[i]}\r\n";

			Disk2.WriteToProgramFiles("Random", "csv", csv, false);
			Logger.Log("Done.");

			void Do2(int horizon)
			{
				float[] der = new float[graphL.Count];
				for (int i = horizon; i < graphL.Count; i++)
					der[i] = graphL[i] - graphL[i - horizon];

				float max = Math.Max(der.Max(), -der.Min());
				Logger.Log($"Max: {max}");

				float[] distr = new float[300];

				for (int i = 1; i < graphL.Count; i++)
				{
					int id = (int)((der[i] / max * 0.95f) * 150 + 150);
					distr[id]++;
				}

				string csv = "";

				for (int i = 0; i < distr.Length; i++)
					csv += $"{distr[i]}\r\n";

				Disk2.WriteToProgramFiles($"GraphDistribution {horizon}", "csv", csv, false);
				Logger.Log("Done.");
			}
		}

		public static void OneSecToTwoSec()
		{
			var graphFolder = "Graph//ForTraining";
			var files = Directory.GetFiles(Disk2._programFiles + graphFolder);

			for (int f = 0; f < files.Count(); f++)
			{
				var graphL = new List<float>();

				Logger.Log(files[f]);

				string[] lines = File.ReadAllLines(files[f]);

				for (int l = 0; l < lines.Length - 1; l += 2)
				{
					float value = (Convert.ToSingle(lines[l], CultureInfo.InvariantCulture) + Convert.ToSingle(lines[l + 1], CultureInfo.InvariantCulture)) / 2f;
					graphL.Add(value);
				}

				string csv = "";

				for (int i = 0; i < graphL.Count; i++)
					csv += $"{graphL[i]}\r\n";

				Disk2.WriteToProgramFiles(Text2.StringAfterLast(files[f], "\\"), "csv", csv, false);				
			}

			Logger.Log("Done.");
		}
	}
}

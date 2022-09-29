using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AbsurdMoneySimulations.Logger;

namespace AbsurdMoneySimulations
{
	public static class NNTester
	{
		public const int testsCount = 2000;

		private static float[] unnormalizedgrafic;
		public static float[] grafic; //[-1, 1]
		public static List<int> availableGraficPoints;
		public static float[][] tests;
		public static float[] answers;



		public static List<float> realGrafic;

		public static void LoadGrafic()
		{
			var files = Directory.GetFiles(Disk.programFiles + "Grafic");
			var graficL = new List<float>();
			availableGraficPoints = new List<int>();

			int g = 0;

			for (int f = 0; f < files.Length; f++)
			{
				string[] lines = File.ReadAllLines(files[f]);

				int l = 1;
				while (l < lines.Length)
				{
					graficL.Add(Convert.ToSingle(lines[l]) - Convert.ToSingle(lines[l - 1]));

					if (l < lines.Length - NN.inputWindow - NN.horizon - 2)
						availableGraficPoints.Add(g);

					l++; g++;
				}

				Log($"Load: \"{TextMethods.StringInsideLast(files[f], "\\", ".")}\"");
			}

			unnormalizedgrafic = graficL.ToArray();
			grafic = new float[unnormalizedgrafic.Length];

			for (int i = 0; i < grafic.Length; i++)
				grafic[i] = Extensions.Normalize(unnormalizedgrafic[i]);


			Log("Grafic (discrete) for education loaded.");
			Log("Grafic is normalized.");
			Log("Also available grafic points are loaded.");
			Log("Grafic length: " + grafic.Length);
		}

		public static void FillTests()
		{
			int maximalDelta = availableGraficPoints.Count();
			float delta_delta = 0.990f * maximalDelta / testsCount;

			tests = new float[testsCount][];
			answers = new float[testsCount];

			int i = 0;
			for (float delta = 0; delta < maximalDelta && i < testsCount; delta += delta_delta)
			{
				int offset = availableGraficPoints[Convert.ToInt32(delta)];

				tests[i] = Extensions.SubArray(grafic, offset, NN.inputWindow);
				
				float[] ar = Extensions.SubArray(unnormalizedgrafic, offset + NN.inputWindow, NN.horizon);
				for (int j = 0; j < ar.Length; j++)
					answers[i] += ar[j];

				i++;
			}

			Log($"Tests and answers for NN are filled. ({tests.Length})");
		}
	}
}

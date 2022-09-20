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

				Log($"Загружен график: \"{TextMethods.StringInsideLast(files[f], "\\", ".")}\"");
			}

			unnormalizedgrafic = graficL.ToArray();
			grafic = new float[unnormalizedgrafic.Length];

			for (int i = 0; i < files.Length; i++)
				grafic[i] = Brain.Normalize(unnormalizedgrafic[i]);


			Log("График (сборный) для обучения загружен.");
			Log("График нормализирован.");
			Log("По совместимости также загружены доступные точки на графике.");
			Log("Длина графика: " + grafic.Length);
		}

		public static void FillTests()
		{
			int maximalDelta = availableGraficPoints.Count();
			float delta_delta = 1.00f * maximalDelta / testsCount;

			tests = new float[testsCount][];

			int i = 0;
			for (float delta = 0; delta < maximalDelta; delta += delta_delta)
			{
				int offset = availableGraficPoints[Convert.ToInt32(delta)];

				tests[i] = Brain.SubArray(grafic, offset, NN.inputWindow);
				
				float[] ar = Brain.SubArray(unnormalizedgrafic, offset + NN.inputWindow, NN.horizon);
				for (int j = 0; j < ar.Length; j++)
					answers[i] += ar[j];

				i++;
			}

			Log($"Тесты и ответы для нейросети заполнены. ({tests.Length})");
		}
	}
}

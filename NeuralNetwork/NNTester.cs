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
					graficL.Add(Brain.Normalize(Convert.ToSingle(lines[l]) - Convert.ToSingle(lines[l - 1])));

					if (l < lines.Length - NN.inputWindow - NN.horizon - 2)
						availableGraficPoints.Add(g);

					l++; g++;
				}

				Log($"Загружен график: \"{TextMethods.StringInsideLast(files[f], "\\", ".")}\"");
			}

			grafic = graficL.ToArray();

			Log("График (сборный) для обучения загружен.");
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
				i++;
			}

			Log($"Тесты для нейросети заполнены. ({tests.Length})");
		}

		public static void FillAnswersForTests()
		{
			answers = new float[grafic.Length];
			for (int i = NN.inputWindow - 1; i < grafic.Length - NN.horizon - 1; i++)
				for (int j = 1; j <= NN.horizon; j++)
					answers[i] += grafic[i + j];

			Log("Ответы для тестов нейросети заполнены.");
		}
	}
}

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

		private static float[] originalGrafic;
		private static float[] derivativeOfGrafic;
		private static float[] normalizedDerivativeOfGrafic; //[-1, 1]

		public static List<int> availableGraficPoints;
		public static float[][] tests;
		public static float[] answers;

		public static void InitForEvolution()
		{
			InitForEvolutionFromNormalizedOriginalGrafic();
		}

		public static void InitForTesting()
		{
			InitForTestingFromNormalizedOriginalGrafic();
		}

		public static void InitForEvolutionFromNormalizedDerivativeGrafic()
		{
			LoadOrigGrafic("Grafic\\ForEvolution", "EVOLUTION");
			FillDerivativeOfGrafic();
			NormalizeDerivativeOfGrafic();

			FillTestsFromNormilizedDerivativeGrafic();
		}

		private static void InitForTestingFromNormalizedDerivativeGrafic()
		{
			LoadOrigGrafic("Grafic\\ForTesting", "TESTING");
			FillDerivativeOfGrafic();
			NormalizeDerivativeOfGrafic();

			FillTestsFromNormilizedDerivativeGrafic();
		}

		public static void InitForEvolutionFromNormalizedOriginalGrafic()
		{
			LoadOrigGrafic("Grafic\\ForEvolution", "EVOLUTION+");
			FillDerivativeOfGrafic();
			NormalizeDerivativeOfGrafic();

			FillTestsFromOriginalGrafic();
		}

		private static void InitForTestingFromNormalizedOriginalGrafic()
		{
			LoadOrigGrafic("Grafic\\ForTesting", "TESTING+");
			FillDerivativeOfGrafic();
			NormalizeDerivativeOfGrafic();

			FillTestsFromOriginalGrafic();
		}

		private static void LoadOrigGrafic(string graficFolder, string reason)
		{
			var files = Directory.GetFiles(Disk.programFiles + graficFolder);
			var graficL = new List<float>();
			availableGraficPoints = new List<int>();

			int g = 0;

			for (int f = 0; f < files.Length; f++)
			{
				string[] lines = File.ReadAllLines(files[f]);

				int l = 0;
				while (l < lines.Length)
				{
					graficL.Add(Convert.ToSingle(lines[l]));

					if (l < lines.Length - NN.inputWindow - NN.horizon - 2)
						availableGraficPoints.Add(g);

					l++; g++;
				}

				Log($"Loaded grafic: \"{TextMethods.StringInsideLast(files[f], "\\", ".")}\"");
			}

			originalGrafic = graficL.ToArray();
			Log($"Original (and discrete) grafic for {reason} loaded.");
			Log("Also available grafic points are loaded.");
			Log("Grafic length: " + originalGrafic.Length + ".");
		}

		private static void FillDerivativeOfGrafic()
		{
			derivativeOfGrafic = new float[originalGrafic.Length];
			for (int i = 1; i < originalGrafic.Length; i++)
				derivativeOfGrafic[i] = originalGrafic[i] - originalGrafic[i - 1];
			Log("Derivative of grafic is filled.");
		}

		private static void NormalizeDerivativeOfGrafic()
		{
			normalizedDerivativeOfGrafic = new float[originalGrafic.Length];
			for (int i = 1; i < derivativeOfGrafic.Length; i++)
				normalizedDerivativeOfGrafic[i] = ActivationFunctions.Normalize(derivativeOfGrafic[i]);
			Log("Derivative of grafic is normilized.");
		}

		private static void FillTestsFromNormilizedDerivativeGrafic()
		{
			int maximalDelta = availableGraficPoints.Count();
			float delta_delta = 0.990f * maximalDelta / testsCount;

			tests = new float[testsCount][];
			answers = new float[testsCount];

			int i = 0;
			for (float delta = 0; delta < maximalDelta && i < testsCount; delta += delta_delta)
			{
				int offset = availableGraficPoints[Convert.ToInt32(delta)];

				tests[i] = Extensions.SubArray(normalizedDerivativeOfGrafic, offset, NN.inputWindow);

				float[] ar = Extensions.SubArray(derivativeOfGrafic, offset + NN.inputWindow, NN.horizon);
				for (int j = 0; j < ar.Length; j++)
					answers[i] += ar[j];

				i++;
			}

			Log($"Tests and answers for NN are filled from NORMILIZED DERIVATIVE grafic. ({tests.Length})");
		}

		private static void FillTestsFromOriginalGrafic()
		{
			int maximalDelta = availableGraficPoints.Count();
			float delta_delta = 0.990f * maximalDelta / testsCount;

			tests = new float[testsCount][];
			answers = new float[testsCount];

			int test = 0;
			for (float delta = 0; delta < maximalDelta && test < testsCount; delta += delta_delta)
			{
				int offset = availableGraficPoints[Convert.ToInt32(delta)];

				tests[test] = Extensions.SubArray(originalGrafic, offset, NN.inputWindow);
				Normalize(test);

				float[] ar = Extensions.SubArray(derivativeOfGrafic, offset + NN.inputWindow, NN.horizon);
				for (int j = 0; j < ar.Length; j++)
					answers[test] += ar[j];

				test++;
			}

			Log($"Tests and answers for NN are filled from NORMILIZED ORIGINAL grafic. ({tests.Length})");

			void Normalize(int test)
			{
				float min = Extensions.Min(tests[test]);
				float max = Extensions.Max(tests[test]);
				float scale = max - min;

				for (int i = 0; i < NNTester.tests[test].Length; i++)
					tests[test][i] = 2 * (tests[test][i] - min) / scale - 1;
			}
		}

		public static float[] OriginalGrafic
		{
			get
			{
				return originalGrafic;
			}
		}
	}
}

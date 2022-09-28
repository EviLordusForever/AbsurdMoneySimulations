using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class NNStatManager
	{
		public static float er;
		public static List<float[]> sections;
		public static float[] wins;
		public static float[] tests;
		public static float[] scores;

		static NNStatManager()
		{
			sections = new List<float[]>();

			sections.Add(new float[] { -1000, 0 });
			sections.Add(new float[] { 0, 1000 });
			sections.Add(new float[] { -1000, 1000 });

			sections.Add(new float[] { -15, -10 });
			sections.Add(new float[] { -10, -7 });
			sections.Add(new float[] { -7, -5 });
			sections.Add(new float[] { -5, -4 });
			sections.Add(new float[] { -4, -3 });
			sections.Add(new float[] { -3, -2 });
			sections.Add(new float[] { -2, -1 });
			sections.Add(new float[] { -1, 0 });
			sections.Add(new float[] { 0, 1 });
			sections.Add(new float[] { 1, 2 });
			sections.Add(new float[] { 2, 3 });
			sections.Add(new float[] { 3, 4 });
			sections.Add(new float[] { 4, 5 });
			sections.Add(new float[] { 5, 7 });
			sections.Add(new float[] { 7, 10 });
			sections.Add(new float[] { 10, 15 });

			wins = new float[sections.Count];
			tests = new float[sections.Count];
			scores = new float[sections.Count];
		}

		public static string GetStatistics()
		{
			ClearStat();

			for (int test = 0; test < NNTester.testsCount; test++)
			{
				float prediction = NN.Calculate(test, NNTester.tests[test]);

				float reality = NNTester.answers[test];

				bool win = prediction > 0 && reality > 0 || prediction < 0 && reality < 0;

				PlusToStatistics(prediction, win);

				er += MathF.Abs(prediction - reality);
			}

			er /= NNTester.testsCount;

			CalculateScores();

			return StatToString();
		}

		public static void PlusToStatistics(float prediction, bool win)
		{
			for (int section = 0; section < sections.Count; section++)
			{
				if (prediction >= sections[section][0] && prediction <= sections[section][1])
				{
					tests[section]++;
					if (win)
						wins[section]++;
				}
			}
		}

		public static void CalculateScores()
		{
			for (int s = 0; s < sections.Count; s++)
				scores[s] = MathF.Round(wins[s] / tests[s], 3);
		}

		public static void ClearStat()
		{
			for (int s = 0; s < sections.Count; s++)
			{
				wins[s] = 0;
				tests[s] = 0;
				scores[s] = 0;
			}
		}

		static string StatToString()
		{
			string stat = "========================\n";
			for (int section = 0; section < wins.Length; section++)
				stat += $"({sections[section][0]}, {sections[section][1]}): {wins[section]} / {tests[section]} ({scores[section]})\n";
			stat += $"er: {er}\n";
			stat += $"========================\n";
			return stat;
		}
	}
}

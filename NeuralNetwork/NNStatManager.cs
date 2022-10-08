﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AbsurdMoneySimulations.Logger;
using static AbsurdMoneySimulations.Storage;

namespace AbsurdMoneySimulations
{
	public static class NNStatManager
	{
		public static float er;
		public static List<float[]> sections;

		public static float[,] winsPerCore;
		public static float[] wins;
		public static float[,] testsPerCore;
		public static float[] tests;

		public static float[] scores;

		static NNStatManager()
		{
			Init();
		}

		public static void Init()
		{
			sections = new List<float[]>();

			sections.Add(new float[] { -1, 1 });

			sections.Add(new float[] { -1, -0.95f });
			sections.Add(new float[] { -1, -0.9f });
			sections.Add(new float[] { -1, -0.8f });
			sections.Add(new float[] { -1, -0.7f });
			sections.Add(new float[] { -1, -0.6f });
			sections.Add(new float[] { -1, -0.5f });
			sections.Add(new float[] { -1, -0.4f });
			sections.Add(new float[] { -1, -0.3f });
			sections.Add(new float[] { -1, -0.2f });
			sections.Add(new float[] { -1, -0.1f });
			sections.Add(new float[] { -1, 0 });
			sections.Add(new float[] { 0, 1 });
			sections.Add(new float[] { 0.1f, 1 });
			sections.Add(new float[] { 0.2f, 1 });
			sections.Add(new float[] { 0.3f, 1 });
			sections.Add(new float[] { 0.4f, 1 });
			sections.Add(new float[] { 0.5f, 1 });
			sections.Add(new float[] { 0.6f, 1 });
			sections.Add(new float[] { 0.7f, 1 });
			sections.Add(new float[] { 0.8f, 1 });
			sections.Add(new float[] { 0.9f, 1 });
			sections.Add(new float[] { 0.95f, 1 });

			winsPerCore = new float[coresCount, sections.Count];
			wins = new float[sections.Count];
			testsPerCore = new float[coresCount, sections.Count];
			tests = new float[sections.Count];
			scores = new float[sections.Count];
		}

		public static string CalculateStatistics()
		{
			restart:

			ClearStat();

			int testsPerCoreCount = NNTester.testsCount / coresCount;

			float[] suber = new float[coresCount];

			int alive = coresCount;

			Thread[] subThreads = new Thread[coresCount];

			for (int core = 0; core < coresCount; core++)
			{
				subThreads[core] = new Thread(new ParameterizedThreadStart(SubThread));
				subThreads[core].Priority = ThreadPriority.Highest;
				subThreads[core].Start(core);
			}

			void SubThread(object obj)
			{
				int core = (int)obj;

				for (int test = core * testsPerCoreCount; test < core * testsPerCoreCount + testsPerCoreCount; test++)
				{
					float prediction = NN.Calculate(test, NNTester.tests[test]);

					float reality = NNTester.answers[test];

					suber[core] += MathF.Pow(prediction - reality, 2);

					bool win = prediction > 0 && reality > 0 || prediction < 0 && reality < 0;

					PlusToStatistics(core, prediction, win);
				}

				alive--;
			}

			long ms = DateTime.Now.Ticks;
			while (alive > 0)
			{
				if (DateTime.Now.Ticks > ms + 10000 * 1000 * 10)
				{
					Log("THE THREAD IS STACKED");
					for (int core = 0; core < coresCount; core++)
						Log($"Thread / core {core}: {subThreads[core].ThreadState}");
					Log("AGAIN");

					goto restart;
				}
			}


			for (int core = 0; core < coresCount; core++)
				er += suber[core];

			er /= NNTester.testsCount;

			CalculateScores();

			return StatToString();
		}

		public static void PlusToStatistics(int core, float prediction, bool win)
		{
			for (int section = 0; section < sections.Count; section++)
				if (prediction >= sections[section][0] && prediction <= sections[section][1])
				{
					testsPerCore[core, section]++;
					if (win)
						winsPerCore[core, section]++;
				}
		}

		public static void CalculateScores()
		{
			for (int section = 0; section < sections.Count; section++)
			{
				for (int core = 0; core < coresCount; core++)
				{
					wins[section] += winsPerCore[core, section];
					tests[section] += testsPerCore[core, section];
				}
				scores[section] = MathF.Round(wins[section] / tests[section], 3);
			}
		}

		public static void ClearStat()
		{
			for (int section = 0; section < sections.Count; section++)
			{
				wins[section] = 0;
				tests[section] = 0;
				scores[section] = 0;

				for (int core = 0; core < coresCount; core++)
				{
					winsPerCore[core, section] = 0;
					testsPerCore[core, section] = 0;
				}
			}

			er = 0;
		}

		static string StatToString()
		{
			string stat = "========================\n";
			for (int section = 0; section < wins.Length; section++)
				stat += $"({sections[section][0]}, {sections[section][1]}): {wins[section]} / {tests[section]} ({scores[section]})\n";
			stat += $"er_fb: {er}\n";
			stat += $"========================";
			return stat;
		}

		public static string StatToCsv(string name)
		{
			string stat = name + ",";
			for (int section = 0; section < wins.Length; section++)
				stat += $"{scores[section]},";
			return stat;
		}
	}
}

﻿using static AbsurdMoneySimulations.Logger;
using static AbsurdMoneySimulations.Storage;

namespace AbsurdMoneySimulations
{
	public static class Stat
	{
		public static float _er;
		public static List<float[]> _sections;

		public static float[,] _winsPerCore;
		public static float[] _wins;
		public static float[,] _testsPerCore;
		public static float[] _tests;

		public static float[] _scores;

		static Stat()
		{
			Init();
		}

		public static void Init()
		{
			_sections = new List<float[]>();

			_sections.Add(new float[] { -1, 1 });

			_sections.Add(new float[] { -1, -0.95f });
			_sections.Add(new float[] { -1, -0.9f });
			_sections.Add(new float[] { -1, -0.8f });
			_sections.Add(new float[] { -1, -0.7f });
			_sections.Add(new float[] { -1, -0.6f });
			_sections.Add(new float[] { -1, -0.5f });
			_sections.Add(new float[] { -1, -0.4f });
			_sections.Add(new float[] { -1, -0.3f });
			_sections.Add(new float[] { -1, -0.2f });
			_sections.Add(new float[] { -1, -0.1f });
			_sections.Add(new float[] { -1, 0 });
			_sections.Add(new float[] { 0, 1 });
			_sections.Add(new float[] { 0.1f, 1 });
			_sections.Add(new float[] { 0.2f, 1 });
			_sections.Add(new float[] { 0.3f, 1 });
			_sections.Add(new float[] { 0.4f, 1 });
			_sections.Add(new float[] { 0.5f, 1 });
			_sections.Add(new float[] { 0.6f, 1 });
			_sections.Add(new float[] { 0.7f, 1 });
			_sections.Add(new float[] { 0.8f, 1 });
			_sections.Add(new float[] { 0.9f, 1 });
			_sections.Add(new float[] { 0.95f, 1 });

			_winsPerCore = new float[_coresCount, _sections.Count];
			_wins = new float[_sections.Count];
			_testsPerCore = new float[_coresCount, _sections.Count];
			_tests = new float[_sections.Count];
			_scores = new float[_sections.Count];
		}

		public static string CalculateStatistics(Tester tester)
		{
			restart:

			ClearStat();

			int testsPerCoreCount = tester._testsCount / _coresCount;

			float[] suber = new float[_coresCount];

			int alive = _coresCount;

			Thread[] subThreads = new Thread[_coresCount];

			for (int core = 0; core < _coresCount; core++)
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
					float prediction = NN.Calculate(test, tester._tests[test]);

					float reality = tester._answers[test];

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
					for (int core = 0; core < _coresCount; core++)
						Log($"Thread / core {core}: {subThreads[core].ThreadState}");
					Log("AGAIN");

					goto restart;
				}
			}


			for (int core = 0; core < _coresCount; core++)
				_er += suber[core];

			_er /= tester._testsCount;

			CalculateScores();

			return StatToString();
		}

		public static void PlusToStatistics(int core, float prediction, bool win)
		{
			for (int section = 0; section < _sections.Count; section++)
				if (prediction >= _sections[section][0] && prediction <= _sections[section][1])
				{
					_testsPerCore[core, section]++;
					if (win)
						_winsPerCore[core, section]++;
				}
		}

		public static void CalculateScores()
		{
			for (int section = 0; section < _sections.Count; section++)
			{
				for (int core = 0; core < _coresCount; core++)
				{
					_wins[section] += _winsPerCore[core, section];
					_tests[section] += _testsPerCore[core, section];
				}
				_scores[section] = MathF.Round(_wins[section] / _tests[section], 3);
			}
		}

		public static void ClearStat()
		{
			for (int section = 0; section < _sections.Count; section++)
			{
				_wins[section] = 0;
				_tests[section] = 0;
				_scores[section] = 0;

				for (int core = 0; core < _coresCount; core++)
				{
					_winsPerCore[core, section] = 0;
					_testsPerCore[core, section] = 0;
				}
			}

			_er = 0;
		}

		static string StatToString()
		{
			string stat = "========================\n";
			for (int section = 0; section < _wins.Length; section++)
				stat += $"({_sections[section][0]}, {_sections[section][1]}): {_wins[section]} / {_tests[section]} ({_scores[section]})\n";
			stat += $"er_fb: {_er}\n";
			stat += $"========================";
			return stat;
		}

		public static string StatToCsv(string name)
		{
			string stat = name + ",";
			for (int section = 0; section < _wins.Length; section++)
				stat += $"{_scores[section]},";
			return stat;
		}
	}
}

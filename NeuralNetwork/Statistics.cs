using static AbsurdMoneySimulations.Logger;
using static AbsurdMoneySimulations.Storage;
using Library;

namespace AbsurdMoneySimulations
{
	public static class Statistics
	{
		public static float _loss;
		public static List<Section> _sections;

		public static int[,] _winsPerCore;
		public static int[] _wins;
		public static int[,] _testsPerCore;
		public static int[] _tests;

		public static float[] _scores;
		public static double[] _randomnesses;

		static Statistics()
		{
			Init();
		}

		public static void Init()
		{
			_sections = new List<Section>();

			_sections.Add(new Section(new float[][] { new float[] { -1, 1 } }));

			_sections.Add(new Section(new float[][] { new float[] { -1, -0.9f }, new float[] { 0.9f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.85f }, new float[] { 0.85f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.8f }, new float[] { 0.8f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.7f }, new float[] { 0.7f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.6f }, new float[] { 0.6f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.5f }, new float[] { 0.5f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.4f }, new float[] { 0.4f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.3f }, new float[] { 0.3f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.2f }, new float[] { 0.2f, 1 } }));
			_sections.Add(new Section(new float[][] { new float[] { -1, -0.1f }, new float[] { 0.1f, 1 } }));

			_winsPerCore = new int[_coresCount, _sections.Count];
			_wins = new int[_sections.Count];
			_testsPerCore = new int[_coresCount, _sections.Count];
			_tests = new int[_sections.Count];
			_scores = new float[_sections.Count];
			_randomnesses = new double[_sections.Count];
		}

		public static string CalculateStatistics(NN nn, Tester tester)
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
					float prediction = nn.Calculate(test, tester._tests[test]);

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
				_loss += suber[core];

			_loss /= tester._testsCount;

			CalculateScores();
			CalculateCDFs();

			return StatToString();
		}

		public static void PlusToStatistics(int core, float prediction, bool win)
		{
			for (int section = 0; section < _sections.Count; section++)
				if (_sections[section].IsInSection(prediction))
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

				_scores[section] = MathF.Round((float)_wins[section] / _tests[section], 3);
			}
		}

		public static void CalculateCDFs()
		{
			for (int section = 0; section < _sections.Count; section++)
			{
				if (_wins[section] > _tests[section] / 2f)
					_randomnesses[section] = 1 - Math2.CumulativeDistributionFunction(_wins[section], _tests[section], 0.5);
				else
					_randomnesses[section] = 1 - Math2.CumulativeDistributionFunction(_tests[section] - _wins[section], _tests[section], 0.5);
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

			_loss = 0;
		}

		static string StatToString()
		{
			string stat = "========================\n";
			for (int section = 0; section < _wins.Length; section++)
			{
				stat += String.Format("{0,-25} {1,-12} {2,-17} (randomness: {3})\n", $"{_sections[section].ToString()}:", $"{_wins[section]} / {_tests[section]}", $"(winrate: {_scores[section]})", string.Format("{0:F9}", _randomnesses[section]));
			}
			stat += $"loss: {_loss}\n";
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

	public class Section
	{
		public List<float[]> _subSections;

		public Section(params float[][] sections)
		{
			_subSections = new List<float[]>();
			foreach (float[] s in sections)
				Add(s);
		}

		public void Add(float[] section)
		{
			_subSections.Add(section);
		}

		public bool IsInSection(float number)
		{
			for (int s = 0; s < _subSections.Count(); s++)
				if (number >= _subSections[s][0] && number <= _subSections[s][1])
					return true;

			return false;
		}

		public override string ToString()
		{
			string str = "";
			for (int s = 0; s < _subSections.Count(); s++)
				str += $"({_subSections[s][0]}, {_subSections[s][1]}), ";
			return str.Remove(str.Length - 2);
		}
	}
}

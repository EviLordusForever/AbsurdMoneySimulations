using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AbsurdMoneySimulations.Logger;

namespace AbsurdMoneySimulations
{
	public static class Tests
	{
		public static void StartTest()
		{
			Thread myThread = new Thread(TestNeuralBattle);
			myThread.Start();			
		}

		public static void TestCreateSaveLoad()
		{
			NN.Create();
			NN.Init();
			NN.Save();
			NN.Load();
		}

		public static void TestCalculateRecalculate()
		{
			NN.Create();
			NN.Init();

			NNTester.LoadGrafic();
			NNTester.FillTests();

			Log("LML: " + NN.lastMutatedLayer);
			int goods = 0;
			int bads = 0;
			int neutral = 0;

			for (int test = 0; test < NNTester.testsCount; test++)
			{
				float before = NN.Calculate(test, NNTester.tests[test]);
				NN.Mutate();
				float after1 = NN.Recalculate(test);
				float after2 = NN.Calculate(test, NNTester.tests[test]);
				NN.Demutate();
				float again1 = NN.Recalculate(test);
				float again2 = NN.Calculate(test, NNTester.tests[test]);

				bool good = (before == again1 && before == again2) && (after1 == after2);

				if (before != after1)
					if (good)
						goods++;
					else
						bads++;
				else neutral++;

			}

			Log($"goods {goods}");
			Log($"bads {bads}");
			Log($"neutral {neutral}");
		}

		public static void TestSpeed()
		{
			NN.Create();
			NN.Init();

			NNTester.LoadGrafic();
			NNTester.FillTests();

			int goods = 0;
			int bads = 0;
			int neutral = 0;

			Log($"==================================");

			float res = 0;

			Log($"Started calculate tests");
			for (int test = 0; test < NNTester.testsCount; test++)
				res += NN.Calculate(test, NNTester.tests[test]);
			Log($"Ended calculate tests");
			Log(res);

			Log($"==================================");
			NN.Mutate();
			Log($"==================================");

			res = 0;

			Log($"Started recalculate tests");
			for (int test = 0; test < NNTester.testsCount; test++)
				res += NN.Recalculate(test);

			Log($"Ended recalculate tests");
			Log(res);

			Log($"==================================");

			res = 0;

			Log($"Started calculate tests");
			for (int test = 0; test < NNTester.testsCount; test++)
				res += NN.Calculate(test, NNTester.tests[test]);
			Log($"Ended calculate tests");
			Log(res);

			Log($"==================================");
			NN.Demutate();
			Log($"==================================");

			res = 0;

			Log($"Started recalculate tests");
			for (int test = 0; test < NNTester.testsCount; test++)
				res += NN.Recalculate(test);

			Log($"Ended recalculate tests");
			Log(res);

			Log($"==================================");

			res = 0;

			Log($"Started calculate tests");
			for (int test = 0; test < NNTester.testsCount; test++)
				res += NN.Calculate(test, NNTester.tests[test]);
			Log($"Ended calculate tests");
			Log(res);
		}

		public static void TestTests()
		{
			string csv = "";
			string[] subcsv = new string[NNTester.testsCount];
			NNTester.LoadGrafic();
			NNTester.FillTests();

			for (int test = 0; test < NNTester.testsCount; test++)
			{
				subcsv[test] = String.Join("\r\n", NNTester.tests[test]);

				subcsv[test] += "\r\n\r\n\r\n";
				subcsv[test] += NNTester.answers[test];
				subcsv[test] += "\r\n\r\n\r\n";
				
				if (test % 50 == 0)
				Log($"t{test}");
			}

			csv = String.Concat(subcsv);

			File.WriteAllText(Disk.programFiles + "tests.csv", csv);
		}

		public static void TestAvailableGP()
		{
			NNTester.LoadGrafic();

			string csv = "";
			string[] subcsv = new string[NNTester.grafic.Length];

			for (int i = 0; i < NNTester.grafic.Length; i++)
				subcsv[i] = NNTester.grafic[i] + ",0\r\n";

			for (int i = 0; i < NNTester.availableGraficPoints.Count; i++)
				subcsv[NNTester.availableGraficPoints[i]] = NNTester.grafic[NNTester.availableGraficPoints[i]] + ",1\r\n";

			csv = string.Concat(subcsv);

			File.WriteAllText(Disk.programFiles + "tests.csv", csv);

			Log("Done!");
		}

		public static void TestEvolution()
		{
			//NN.Create();
			//NN.Save();
			NN.Load();
			NN.Init();

			NNTester.LoadGrafic();
			NNTester.FillTests();

			NN.Evolve();
		}

		public static void TestNeuralBattle()
		{
			NN.NeuralBattle();
		}
	}
}

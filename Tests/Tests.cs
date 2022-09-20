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
			Thread myThread = new Thread(TestSpeed);
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
			NNTester.FillAnswersForTests();

			NN.SelectLayerForMutation();
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
			NNTester.FillAnswersForTests();

			NN.SelectLayerForMutation();

			Log("LML: " + NN.lastMutatedLayer);
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
	}
}

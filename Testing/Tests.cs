using OpenQA.Selenium;
using static AbsurdMoneySimulations.BrowserManager;
using static AbsurdMoneySimulations.Logger;
using Library;

namespace AbsurdMoneySimulations
{
	public static class Tests
	{
		public static void StartTest()
		{
			Thread myThread = new Thread(TestTests);
			myThread.Name = "Tests thread";
			myThread.Start();
		}

		public static void TestCreateSaveLoad()
		{
			NN nn = NN.CreateBasicNN();
			NN.Save(nn);
			nn = NN.Load();
		}

		public static void TestTests()
		{
			NN nn = NN.Load();
			string csv = "";
			string[] subcsv = new string[nn._testerE._testsCount];

			for (int test = 0; test < nn._testerE._testsCount; test++)
			{
				subcsv[test] = String.Join("\r\n", nn._testerE._tests[test]);

				subcsv[test] += "\r\n\r\n\r\n";
				subcsv[test] += nn._testerE._answers[test];
				subcsv[test] += "\r\n\r\n\r\n";

				if (test % 50 == 0)
					Log($"t{test}");
			}

			csv = String.Concat(subcsv);

			File.WriteAllText(Disk2._programFiles + "tests.csv", csv);
		}

		public static void TestAvailableGP()
		{
			NN nn = NN.Load();

			string csv = "";
			string[] subcsv = new string[nn._testerE.OriginalGrafic.Length];

			for (int i = 0; i < nn._testerE.OriginalGrafic.Length; i++)
				subcsv[i] = nn._testerE.OriginalGrafic[i] + ",0\r\n";

			for (int i = 0; i < nn._testerE._availableGraficPoints.Count; i++)
				subcsv[nn._testerE._availableGraficPoints[i]] = nn._testerE.OriginalGrafic[nn._testerE._availableGraficPoints[i]] + ",1\r\n";

			csv = string.Concat(subcsv);

			File.WriteAllText(Disk2._programFiles + "tests.csv", csv);

			Log("Done!");
		}

		public static void TestNeuralBattle()
		{
			Manager.NeuralBattle();
		}

		public static void StupiedTest()
		{
			NN nn = NN.CreateBasicNN();

			for (int i = 0; i < 100; i++)
				Log(nn.FindLossSquared(nn._testerE, false));
		}

		public static void TestSelenium()
		{
			LoadBrowser("https://google.com");
			Thread.Sleep(3000);
			Navi("https://vk.com");
			Thread.Sleep(3000);
			Navi("https://yandex.com");
			Thread.Sleep(1000);
			Navi("https://reddit.com");
		}

		public static void TestSelenium2()
		{
			LoadBrowser("https://google.com");
			UserAsker.SayWait("So, let's we begin");
			Thread.Sleep(100);
			_driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).Click();
			_driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).SendKeys("KAPIBARA");
			_driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).SendKeys(OpenQA.Selenium.Keys.Enter);
		}

		public static void TestCoresCountSpeed()
		{
			NN nn = NN.Load();

			So(1);
			So(2);
			So(4);
			So(8);
			So(16);
			So(50);

			void So(int coresCount)
			{
				Storage._coresCount = coresCount;
				long ms = DateTime.Now.Ticks;
				for (int i = 0; i < 10; i++)
					nn.FindLossSquared(nn._testerE, false);
				Log($"{Storage._coresCount} cores: {(decimal)(DateTime.Now.Ticks - ms) / (10000 * 1000)}");
			}
		}

		public static void TestMegatronLayers()
		{
			NN nn = NN.Load();

			nn._testerE.FillTestsFromNormilizedDerivativeGrafic();

			int test = Math2.rnd.Next(nn._testerE._testsCount);
			string[] strings = new string[nn._inputWindow];

			for (int i = 0; i < nn._testerE._tests[test].Length; i++)
				strings[i] += nn._testerE._tests[test][i].ToString();

			nn._testerE.FillTestsFromOriginalGrafic();

			for (int i = 0; i < nn._testerE._tests[test].Length; i++)
				strings[i] += "," + nn._testerE._tests[test][i].ToString();

			nn = NN.Load();

			nn.Calculate(test, nn._testerE._tests[test], false);

			int subsCount = nn._layers[0]._values[test].Count();
			float inputSize = 300;
			int head = 30;
			int d = 2;
			float subSize = (inputSize - head) / d + 1;

			for (int sub = 0; sub < subsCount; sub++)
			{
				for (int v = 0 + head; v < nn._testerE._tests[test].Length - head; v++)
				{
					int j = (int)(((v - subsCount) / (inputSize - head)) * subSize);
					strings[v] += "," + nn._layers[0]._values[test][sub][j];
				}
			}

			Disk2.WriteToProgramFiles("MegatronTest", "csv", string.Join('\n', strings), false);
			Log("done");
		}

		public static void TestRefs()
		{
			float a = 111;
			ref float b = ref a;
			Log($"a = {a}; b = {b}.");
			a += 5;
			Log($"a = {a}; b = {b}.");
			b += 5;
			Log($"a = {a}; b = {b}.");

			float[] ar = new float[] { 60, 61, 62, 63, 64, 65 };

			Log($"ar {ar[0]} {ar[1]} {ar[2]} {ar[3]} {ar[4]} {ar[5]}.");

			ref float c = ref ar[2];

			Log($"ar {ar[0]} {ar[1]} {ar[2]} {ar[3]} {ar[4]} {ar[5]}.");
			Log($"c {c}");

			c += 5;

			Log($"ar {ar[0]} {ar[1]} {ar[2]} {ar[3]} {ar[4]} {ar[5]}.");
			Log($"c {c}");

			ar[2] -= 3;
			ar[3] -= 3;

			Log($"ar {ar[0]} {ar[1]} {ar[2]} {ar[3]} {ar[4]} {ar[5]}.");
			Log($"c {c}");
		}

		public static void TestRecursy()
		{
			NN nn = NN.Load();

			nn._layers = new List<Layer>();
			nn._layers.Add(new LayerPerceptron(nn, nn._testerE._testsCount, 5, 5, 0, new Linear()));

			nn._layers[0].FillWeightsRandomly();

			float[] ar = new float[] { 1, 1, 1, 1, 1 };

			nn._layers[0].Calculate(0, ar, false);
			Log(nn._layers[0].GetAnswer(0));

			for (int i = 0; i < 20; i++)
			{
				nn._layers[0].Calculate(0, nn._layers[0].GetValues(0), false);
				Log(nn._layers[0].GetAnswer(0));
			}
		}

		public static void TestCombinations()
		{
			Log(Math2.Combinations2(5, 1));
			Log(Math2.Combinations2(5, 2));
			Log(Math2.Combinations2(5, 3));
			Log(Math2.Combinations2(5, 4));
			Log(Math2.Combinations2(5, 5));
			Log(Math2.Combinations2(13, 5));


			Log(Math2.Combinations1(5, 1));
			Log(Math2.Combinations1(5, 2));
			Log(Math2.Combinations1(5, 3));
			Log(Math2.Combinations1(5, 4));
			Log(Math2.Combinations1(5, 5));
			Log(Math2.Combinations1(13, 5));
			Log(Math2.Combinations1(20, 5));

			Log($"==============");
			Log($"At least 5 from 5 {Math2.CumulativeDistributionFunction(5, 5, 0.5)}");
			Log($"At least 2 from 4 {Math2.CumulativeDistributionFunction(2, 4, 0.5)}");
			Log($"At least 5 from 10 {Math2.CumulativeDistributionFunction(5, 10, 0.5)}");
			Log($"At least 500 from 1000 {Math2.CumulativeDistributionFunction(500, 1000, 0.5)}");
			Log($"At least 499 from 1000 {Math2.CumulativeDistributionFunction(499, 1000, 0.5)}");
			Log($"At least 1 from 5 {Math2.CumulativeDistributionFunction(1, 5, 0.5)}");
			Log($"At least 2 from 5 {Math2.CumulativeDistributionFunction(2, 5, 0.5)}");
			Log($"At least 3 from 5 {Math2.CumulativeDistributionFunction(3, 5, 0.5)}");

			int yes = 0;
			int no = 0;
			for (int i = 0; i < 100000; i++)
			{
				int count = 0;
				for (int p = 0; p < 4; p++)
					if (Math2.rnd.Next(2) == 1)
						count++;

				if (count >= 2)
					yes++;
				else
					no++;
			}

			Log($"yes {yes}");
			Log($"no {no}");

			Log("==================");
			string csv = "";

			int n = 100;

			var a = new MathNet.Numerics.Distributions.Binomial(0.5, n);

			for (int k = 0; k <= n; k++)
			{
				csv += a.Probability(k) * 13 + ",";
				csv += Math2.CalculateRandomness(k, n) + "\n";
				Log(Math2.CalculateRandomness(k, n));
			}

			Disk2.WriteToProgramFiles("Cumulative", ".csv", csv, false);
			Log($"done");
		}
	}
}

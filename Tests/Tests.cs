using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AbsurdMoneySimulations.Logger;
using static AbsurdMoneySimulations.BrowserManager;
using OpenQA.Selenium;
using static AbsurdMoneySimulations.Extensions;
using static AbsurdMoneySimulations.NN;

namespace AbsurdMoneySimulations
{
	public static class Tests
	{
		public static void StartTest()
		{
			Thread myThread = new Thread(TestMegatronLayers);
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

			Log("LML: " + NN.lastMutatedLayer);
			int goods = 0;
			int bads = 0;
			int neutral = 0;

			for (int test = 0; test < NN.testerE.testsCount; test++)
			{
				float before = Calculate(test, testerE, testerE.tests[test]);
				Mutate();
				float after1 = Recalculate(test, testerE);
				float after2 = Calculate(test, testerE, testerE.tests[test]);
				Demutate();
				float again1 = Recalculate(test, testerE);
				float again2 = Calculate(test, testerE, testerE.tests[test]);

				bool good = (before == again1 && before == again2) && (after1 == after2);

				if (good)
					if (before != after1)
						goods++;
					else neutral++;
				else
				{
					bads++;
					Log($"ERROR: {before} {after1} {after2} {again1} {again2}");
				}
			}

			Log($"goods {goods}");
			Log($"bads {bads}");
			Log($"neutral {neutral}");
		}

		public static void TestSpeed()
		{
			NN.Create();
			NN.Init();

			int goods = 0;
			int bads = 0;
			int neutral = 0;

			Log($"==================================");

			float res = 0;

			Log($"Started calculate tests");
			for (int test = 0; test < testerE.testsCount; test++)
				res += NN.Calculate(test, testerE, testerE.tests[test]);
			Log($"Ended calculate tests");
			Log(res);

			Log($"==================================");
			NN.Mutate();
			Log($"==================================");

			res = 0;

			Log($"Started recalculate tests");
			for (int test = 0; test < NN.testerE.testsCount; test++)
				res += NN.Recalculate(test, NN.testerE);

			Log($"Ended recalculate tests");
			Log(res);

			Log($"==================================");

			res = 0;

			Log($"Started calculate tests");
			for (int test = 0; test < NN.testerE.testsCount; test++)
				res += NN.Calculate(test, testerE, testerE.tests[test]);
			Log($"Ended calculate tests");
			Log(res);

			Log($"==================================");
			NN.Demutate();
			Log($"==================================");

			res = 0;

			Log($"Started recalculate tests");
			for (int test = 0; test < NN.testerE.testsCount; test++)
				res += NN.Recalculate(test, NN.testerE);

			Log($"Ended recalculate tests");
			Log(res);

			Log($"==================================");

			res = 0;

			Log($"Started calculate tests");
			for (int test = 0; test < testerE.testsCount; test++)
				res += Calculate(test, testerE, testerE.tests[test]);
			Log($"Ended calculate tests");
			Log(res);
		}

		public static void TestTests()
		{
			string csv = "";
			string[] subcsv = new string[NN.testerE.testsCount];

			for (int test = 0; test < NN.testerE.testsCount; test++)
			{
				subcsv[test] = String.Join("\r\n", NN.testerE.tests[test]);

				subcsv[test] += "\r\n\r\n\r\n";
				subcsv[test] += NN.testerE.answers[test];
				subcsv[test] += "\r\n\r\n\r\n";
				
				if (test % 50 == 0)
				Log($"t{test}");
			}

			csv = String.Concat(subcsv);

			File.WriteAllText(Disk.programFiles + "tests.csv", csv);
		}

		public static void TestAvailableGP()
		{
			string csv = "";
			string[] subcsv = new string[NN.testerE.OriginalGrafic.Length];

			for (int i = 0; i < NN.testerE.OriginalGrafic.Length; i++)
				subcsv[i] = NN.testerE.OriginalGrafic[i] + ",0\r\n";

			for (int i = 0; i < NN.testerE.availableGraficPoints.Count; i++)
				subcsv[NN.testerE.availableGraficPoints[i]] = NN.testerE.OriginalGrafic[NN.testerE.availableGraficPoints[i]] + ",1\r\n";

			csv = string.Concat(subcsv);

			File.WriteAllText(Disk.programFiles + "tests.csv", csv);

			Log("Done!");
		}

		public static void TestNeuralBattle()
		{
			NN.NeuralBattle();
		}

		public static void StupiedTest()
		{
			NN.Create();
			NN.Init();
			NN.Mutate();

			for (int i =0; i < 100; i++)
				Log(NN.FindErrorRateSquared(NN.testerE));
		}

		public static void StupiedTestR()
		{
			NN.Create();
			NN.Init();
			NN.Mutate();

			Log("er_fb " + NN.FindErrorRateSquared(NN.testerE));

			for (int i = 0; i < 100; i++)
				Log("er_nfb " + NN.RefindErrorRateSquared(NN.testerE));
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
			driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).Click();
			driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).SendKeys("KAPIBARA");
			driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).SendKeys(OpenQA.Selenium.Keys.Enter);
		}

		public static void TestCoresCountSpeed()
		{
			NN.Load();
			NN.Init();

			So(1);
			So(2);
			So(4);
			So(8);
			So(16);
			So(50);

			void So(int coresCount)
			{
				Storage.coresCount = coresCount;
				long ms = DateTime.Now.Ticks;
				for (int i = 0; i < 10; i++)
					NN.FindErrorRateSquared(NN.testerE);
				Log($"{Storage.coresCount} cores: {(decimal)(DateTime.Now.Ticks - ms) / (10000 * 1000)}");
			}
		}

		public static void TestMegatronLayers()
		{
			Load();
			Init();
			testerE.InitFromNormalizedDerivativeGrafic("Grafic\\ForEvolution", "EVOLTION");

			int test = Storage.rnd.Next(testerE.testsCount);
			string[] strings = new string[inputWindow];

			for (int i = 0; i < testerE.tests[test].Length; i++)
				strings[i] += testerE.tests[test][i].ToString();

			testerE.InitFromNormalizedOriginalGrafic("Grafic\\ForEvolution", "EVOLTION");

			for (int i = 0; i < testerE.tests[test].Length; i++)
				strings[i] += "," + testerE.tests[test][i].ToString();

			Load();
			Init();
			Calculate(test, testerE, testerE.tests[test]);

			int subsCount = NN.layers[0].values[test].Count();
			float inputSize = 300;
			int head = 30;
			int d = 1;
			float subSize = (inputSize - head) / d + 1;

			for (int sub = 0; sub < subsCount; sub++)
			{
				for (int v = 0 + head; v < testerE.tests[test].Length - head; v++)
				{
					int j = (int)(((v - subsCount) / (inputSize - head)) * subSize);
					strings[v] += "," + NN.layers[0].values[test][sub][j];
				}
			}

			Disk.WriteToProgramFiles("MegatronTest", "csv", string.Join('\n', strings), false);
			Log("done");
		}

		public static void TestMutations()
		{
			FormsManager.OpenShowForm("normal distribution");
			int width = 800;
			NN.randomMutatesCount = 202200;
			NN.randomMutatesScaleV2 = width / 2;
			NN.FillRandomMutations();
			float[] distribution = new float[width];
			for (int m = 0; m < NN.randomMutates.Count(); m++)
				distribution[Convert.ToInt32(NN.randomMutates[m]*0.99f) + width / 2]++;
			float max = Extensions.Max(distribution);
			Storage.bmp = new Bitmap(width, (int)max);
			Graphics gr = Graphics.FromImage(Storage.bmp);
			for (int i = 0; i < distribution.Count(); i++)
				gr.DrawLine(Pens.Black, i, max, i, max - distribution[i]);
			FormsManager.mainForm.Invoke(new Action(() =>
			{
				FormsManager.showForm.BackgroundImage = Extensions.RescaleBitmap(Storage.bmp, Storage.bmp.Width, FormsManager.showForm.ClientSize.Height);
			}));
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
			InitTesters();
			Create();
			layers = new List<LayerAbstract>();
			layers.Add(new LayerPerceptron(testerE.testsCount, 5, 5));
			Init();
			layers[0].FillWeightsRandomly();

			float[] ar = new float[] { 1, 1, 1, 1, 1 };

			layers[0].Calculate(0, ar);
			Log(layers[0].GetAnswer(0));

			for (int i = 0; i < 20; i++)
			{
				layers[0].Calculate(0, layers[0].GetValues(0));
				Log(layers[0].GetAnswer(0));
			}
		}
	}
}

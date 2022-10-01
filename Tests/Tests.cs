using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AbsurdMoneySimulations.Logger;
using static AbsurdMoneySimulations.BrowserManager;
using OpenQA.Selenium;

namespace AbsurdMoneySimulations
{
	public static class Tests
	{
		public static void StartTest()
		{
			Thread myThread = new Thread(TestMutations);
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

			NNTester.InitForEvolution();

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

			NNTester.InitForEvolution();

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
			NNTester.InitForEvolution();

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
			NNTester.InitForEvolution();

			string csv = "";
			string[] subcsv = new string[NNTester.OriginalGrafic.Length];

			for (int i = 0; i < NNTester.OriginalGrafic.Length; i++)
				subcsv[i] = NNTester.OriginalGrafic[i] + ",0\r\n";

			for (int i = 0; i < NNTester.availableGraficPoints.Count; i++)
				subcsv[NNTester.availableGraficPoints[i]] = NNTester.OriginalGrafic[NNTester.availableGraficPoints[i]] + ",1\r\n";

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

			NNTester.InitForEvolution();

			NN.Evolve();
		}

		public static void TestNeuralBattle()
		{
			NN.NeuralBattle();
		}

		public static void StupiedTest()
		{
			NN.Create();
			NN.Init();
			NNTester.InitForEvolution();
			NN.Mutate();

			for (int i =0; i < 100; i++)
				Log(NN.FindErrorRate());
		}

		public static void StupiedTestR()
		{
			NN.Create();
			NN.Init();
			NNTester.InitForEvolution();
			NN.Mutate();

			Log("er_fb " + NN.FindErrorRate());

			for (int i = 0; i < 100; i++)
				Log("er_nfb " + NN.RefindErrorRate());
		}

		public static void TestTrader()
		{
			LoadBrowser("https://google.com");
			Thread.Sleep(3000);
			Navi("https://vk.com");
			Thread.Sleep(3000);
			Navi("https://yandex.com");
			Thread.Sleep(1000);
			Navi("https://reddit.com");
		}

		public static void TestSelenium()
		{
			LoadBrowser("https://google.com");
			MessageBox.Show("So, let's we begin");
			Thread.Sleep(100);
			driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).Click();
			driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).SendKeys("KAPIBARA");
			driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).SendKeys(OpenQA.Selenium.Keys.Enter);
		}

		public static void TestCoresCountSpeed()
		{
			NN.Load();
			NN.Init();
			NNTester.InitForEvolution();

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
					NN.FindErrorRate();
				Log($"{Storage.coresCount} cores: {(decimal)(DateTime.Now.Ticks - ms) / (10000 * 1000)}");
			}
		}

		public static void TestMegatronLayers()
		{
			NNTester.InitForEvolutionFromNormalizedDerivativeGrafic();

			int test = Storage.rnd.Next(NNTester.testsCount);
			string[] strings = new string[NN.inputWindow];

			for (int i = 0; i < NNTester.tests[test].Length; i++)
				strings[i] += NNTester.tests[test][i].ToString();

			NNTester.InitForEvolutionFromNormalizedOriginalGrafic();

			for (int i = 0; i < NNTester.tests[test].Length; i++)
				strings[i] += "," + NNTester.tests[test][i].ToString();

			//NNTester.InitForEvolution();
			NN.Load();
			NN.Init();
			NN.Calculate(test, NNTester.tests[test]);

			for (int sub = 0; sub < 15; sub++)
			{
				for (int i = 0 + 16; i < NNTester.tests[test].Length - 16; i++)
				{
					int j = (int)Math.Round(((i - 16) / (300.0 - 30.0)) * 55.0);
					strings[i] += "," + NN.layers[0].values[test][sub][j];
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
	}
}

using OpenQA.Selenium;
using static AbsurdMoneySimulations.BrowserManager;
using static AbsurdMoneySimulations.Logger;
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

			Log("LML: " + NN._lastMutatedLayer);
			int goods = 0;
			int bads = 0;
			int neutral = 0;

			for (int test = 0; test < NN._testerE._testsCount; test++)
			{
				float before = Calculate(test, _testerE, _testerE._tests[test]);
				Mutate();
				float after1 = Recalculate(test, _testerE);
				float after2 = Calculate(test, _testerE, _testerE._tests[test]);
				Demutate();
				float again1 = Recalculate(test, _testerE);
				float again2 = Calculate(test, _testerE, _testerE._tests[test]);

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
			for (int test = 0; test < _testerE._testsCount; test++)
				res += NN.Calculate(test, _testerE, _testerE._tests[test]);
			Log($"Ended calculate tests");
			Log(res);

			Log($"==================================");
			NN.Mutate();
			Log($"==================================");

			res = 0;

			Log($"Started recalculate tests");
			for (int test = 0; test < NN._testerE._testsCount; test++)
				res += NN.Recalculate(test, NN._testerE);

			Log($"Ended recalculate tests");
			Log(res);

			Log($"==================================");

			res = 0;

			Log($"Started calculate tests");
			for (int test = 0; test < NN._testerE._testsCount; test++)
				res += NN.Calculate(test, _testerE, _testerE._tests[test]);
			Log($"Ended calculate tests");
			Log(res);

			Log($"==================================");
			NN.Demutate();
			Log($"==================================");

			res = 0;

			Log($"Started recalculate tests");
			for (int test = 0; test < NN._testerE._testsCount; test++)
				res += NN.Recalculate(test, NN._testerE);

			Log($"Ended recalculate tests");
			Log(res);

			Log($"==================================");

			res = 0;

			Log($"Started calculate tests");
			for (int test = 0; test < _testerE._testsCount; test++)
				res += Calculate(test, _testerE, _testerE._tests[test]);
			Log($"Ended calculate tests");
			Log(res);
		}

		public static void TestTests()
		{
			string csv = "";
			string[] subcsv = new string[NN._testerE._testsCount];

			for (int test = 0; test < NN._testerE._testsCount; test++)
			{
				subcsv[test] = String.Join("\r\n", NN._testerE._tests[test]);

				subcsv[test] += "\r\n\r\n\r\n";
				subcsv[test] += NN._testerE._answers[test];
				subcsv[test] += "\r\n\r\n\r\n";

				if (test % 50 == 0)
					Log($"t{test}");
			}

			csv = String.Concat(subcsv);

			File.WriteAllText(Disk._programFiles + "tests.csv", csv);
		}

		public static void TestAvailableGP()
		{
			string csv = "";
			string[] subcsv = new string[NN._testerE.OriginalGrafic.Length];

			for (int i = 0; i < NN._testerE.OriginalGrafic.Length; i++)
				subcsv[i] = NN._testerE.OriginalGrafic[i] + ",0\r\n";

			for (int i = 0; i < NN._testerE._availableGraficPoints.Count; i++)
				subcsv[NN._testerE._availableGraficPoints[i]] = NN._testerE.OriginalGrafic[NN._testerE._availableGraficPoints[i]] + ",1\r\n";

			csv = string.Concat(subcsv);

			File.WriteAllText(Disk._programFiles + "tests.csv", csv);

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

			for (int i = 0; i < 100; i++)
				Log(NN.FindErrorRateSquared(NN._testerE));
		}

		public static void StupiedTestR()
		{
			NN.Create();
			NN.Init();
			NN.Mutate();

			Log("er_fb " + NN.FindErrorRateSquared(NN._testerE));

			for (int i = 0; i < 100; i++)
				Log("er_nfb " + NN.RefindErrorRateSquared(NN._testerE));
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
				Storage._coresCount = coresCount;
				long ms = DateTime.Now.Ticks;
				for (int i = 0; i < 10; i++)
					NN.FindErrorRateSquared(NN._testerE);
				Log($"{Storage._coresCount} cores: {(decimal)(DateTime.Now.Ticks - ms) / (10000 * 1000)}");
			}
		}

		public static void TestMegatronLayers()
		{
			Load();
			Init();
			_testerE.InitFromNormalizedDerivativeGrafic("Grafic\\ForEvolution", "EVOLTION");

			int test = Storage.rnd.Next(_testerE._testsCount);
			string[] strings = new string[_inputWindow];

			for (int i = 0; i < _testerE._tests[test].Length; i++)
				strings[i] += _testerE._tests[test][i].ToString();

			_testerE.InitFromNormalizedOriginalGrafic("Grafic\\ForEvolution", "EVOLTION");

			for (int i = 0; i < _testerE._tests[test].Length; i++)
				strings[i] += "," + _testerE._tests[test][i].ToString();

			Load();
			Init();
			Calculate(test, _testerE, _testerE._tests[test]);

			int subsCount = NN._layers[0]._values[test].Count();
			float inputSize = 300;
			int head = 30;
			int d = 1;
			float subSize = (inputSize - head) / d + 1;

			for (int sub = 0; sub < subsCount; sub++)
			{
				for (int v = 0 + head; v < _testerE._tests[test].Length - head; v++)
				{
					int j = (int)(((v - subsCount) / (inputSize - head)) * subSize);
					strings[v] += "," + NN._layers[0]._values[test][sub][j];
				}
			}

			Disk.WriteToProgramFiles("MegatronTest", "csv", string.Join('\n', strings), false);
			Log("done");
		}

		public static void TestMutations()
		{
			FormsManager.OpenShowForm("normal distribution");
			int width = 800;
			NN._randomMutatesCount = 202200;
			NN._randomMutatesScaleV2 = width / 2;
			NN.FillRandomMutations();
			float[] distribution = new float[width];
			for (int m = 0; m < NN._randomMutates.Count(); m++)
				distribution[Convert.ToInt32(NN._randomMutates[m] * 0.99f) + width / 2]++;
			float max = Extensions.Max(distribution);
			Storage._bmp = new Bitmap(width, (int)max);
			Graphics gr = Graphics.FromImage(Storage._bmp);
			for (int i = 0; i < distribution.Count(); i++)
				gr.DrawLine(Pens.Black, i, max, i, max - distribution[i]);
			FormsManager._mainForm.Invoke(new Action(() =>
			{
				FormsManager._showForm.BackgroundImage = Extensions.RescaleBitmap(Storage._bmp, Storage._bmp.Width, FormsManager._showForm.ClientSize.Height);
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
			_layers = new List<LayerAbstract>();
			_layers.Add(new LayerPerceptron(_testerE._testsCount, 5, 5));
			Init();
			_layers[0].FillWeightsRandomly();

			float[] ar = new float[] { 1, 1, 1, 1, 1 };

			_layers[0].Calculate(0, ar);
			Log(_layers[0].GetAnswer(0));

			for (int i = 0; i < 20; i++)
			{
				_layers[0].Calculate(0, _layers[0].GetValues(0));
				Log(_layers[0].GetAnswer(0));
			}
		}
	}
}

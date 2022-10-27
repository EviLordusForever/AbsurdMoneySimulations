using OpenQA.Selenium;
using System.Threading;
using static AbsurdMoneySimulations.BrowserManager;
using static AbsurdMoneySimulations.Logger;
using Library;
using IronOcr;
using System.Timers;


namespace AbsurdMoneySimulations
{
	public static class Trader
	{
		private static bool _itIsTime;
		private static bool _graphUpdated;
		private static Bitmap bmp;
		private static Graphics gr;

		private static List<float> _graphLive = new List<float>();
		private static List<float> _derivativeLive = new List<float>();
		private static List<float> _horizonLive = new List<float>();
		private static float[] input;

		private static float _prediction;
		private static float _previousPrediction;

		public static void Test()
		{
			LoadBrowser("https://google.com");
			LoadCookies();
			UserAsker.SayWait("So, let's we begin!");
			Thread.Sleep(100);
			_driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).Click();
			_driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).SendKeys("KAPIBARA");
			_driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).SendKeys(OpenQA.Selenium.Keys.Enter);
			SaveCookies();
		}

		public static void TradeBySwarm()
		{
			Swarm.Load();
			int horizon = Swarm.swarm[0]._horizon;
			int inputWindow = Swarm.swarm[0]._inputWindow;
			ActivationFunction inputAF = Swarm.swarm[0]._inputAF;
			int moveInput = Swarm.swarm[0]._testerE._moveInputsOverZero;

			InititializePredictionForm();

			input = new float[inputWindow];
			string traderReport = "";
			int i = 0;

			while (true)
			{				
				WaitGraphUpdated();
				traderReport = "";

				AddToDerivativeLive();
				AddToHorizonLive();

				CutInput();

				if (_horizonLive.Count > inputWindow)
				{
					_previousPrediction = _prediction;
					_prediction = Swarm.Calculate(input);

					traderReport += $"{i}\n";
					traderReport += $"h: {_horizonLive[_horizonLive.Count - 1]}\n";
					traderReport += $"predction: {_prediction}";
					i++;
				}
				else
					traderReport += $"Waiting for {inputWindow - _horizonLive.Count} more points to start predicting\n";

				FormsManager.SayToTraderReport2(traderReport);
				DrawPrediction(horizon);
			}

			void CutInput()
			{
				if (_horizonLive.Count > 1)
				{
					int cuttedWindow = Math.Min(inputWindow, _horizonLive.Count - 1);
					input = _horizonLive.GetRange(_horizonLive.Count - 1 - cuttedWindow, cuttedWindow).ToArray();
					float standartDeviation = Math2.FindStandartDeviation(input);
					input = Tester.Normalize(input, standartDeviation, inputAF, moveInput);
				}
			}

			void AddToDerivativeLive()
			{
				if (_graphLive.Count > 1)
					_derivativeLive.Add(_graphLive[_graphLive.Count - 1] - _graphLive[_graphLive.Count - 2]);
			}

			void AddToHorizonLive()
			{
				if (_graphLive.Count > horizon)
					_horizonLive.Add(_graphLive[_graphLive.Count - 1] - _graphLive[_graphLive.Count - 1 - horizon]);
				else
					traderReport += $"Waiting for {horizon - _graphLive.Count} more points to start finding Horizon\n";
			}

			void WaitGraphUpdated()
			{
				while (!_graphUpdated) { }
				_graphUpdated = false;
			}
		}

		public static void GetGraphLive(int delaySeconds)
		{
			LoadBrowser("https://google.com");
			LoadCookies();
			CloseChromeMessage();
			OpenQtx();
			StaySignedIn();
			MakeGraphBackupCopy();
			//DoMaxScale();
			string graphCSV = "";

			StartSavingGraph(10);
			StartTraderTimer(delaySeconds);
			FormsManager.OpenTraderReportForm();

			int valueLength = FindValueLength();
			string value = "";
			string previousValue = "";
			string info = "";

			for (int i = 0; ; i += delaySeconds)
			{
				WaitYourTime();

				info = "";

				try
				{
					value = GetQtxGraficValue();
				}
				catch (CantFindBlueLabelException ex)
				{
					value = previousValue;
					info = "Can't find blue label skip";
				}

				try
				{
					float valuef = Convert.ToSingle(value);
				}
				catch
				{
					value = previousValue;
					info = "Not float skip";
				}

				if (value.Length != valueLength)
				{
					value = previousValue;
					info = "Wrong length skip";
				}
				else
					previousValue = value;

				//if (i == 10)
				//	FakeFill();

				_graphLive.Add(Convert.ToSingle(value));
				graphCSV += $"{value}\n";
				_graphUpdated = true;
				FormsManager.SayToTraderReport1($"{i}\n{value}\n{info}");

				MakeGraphBackupEvery(i, 3600);
			}

			void FakeFill()
			{
				for (int j = 0; j < 360; j++)
					_graphLive.Add(Convert.ToSingle(value));

				for (int j = 0; j < 290; j++)
					_horizonLive.Add(Math2.rnd.NextSingle() * 0.2f - 0.1f);

				Log("Filled graph by fake numbers");
			}

			void MakeGraphBackupEvery(int seconds, int everyS)
			{
				if (seconds % everyS == everyS - (everyS % delaySeconds) - delaySeconds)
				{
					Thread myThread = new Thread(GraphBackupThread);
					myThread.Name = "Graph Backup Thread";
					myThread.Start();

					void GraphBackupThread()
					{
						MakeGraphBackupCopy();
					}
				}
			}

			void WaitYourTime()
			{
				while (!_itIsTime) { };
				_itIsTime = false;
			}

			void StartSavingGraph(int delaySecons)
			{
				Thread myThread = new Thread(GraphSaverThread);
				myThread.Name = "Graph Saver Thread";
				myThread.Start();

				void GraphSaverThread()
				{
					while (true)
					{
						Thread.Sleep(delaySeconds * 1000);
						Disk2.WriteToProgramFiles("Grafic\\NewGraph", "csv", graphCSV, false);
					}
				}
			}
		}

		public static void DrawPrediction(int horizon)
		{
			int d = 4;

			gr.DrawImage(bmp, -d, 0);
			gr.FillRectangle(Brushes.Black, bmp.Width - d, 0, bmp.Width - 1, bmp.Height);

			Pen darkPen = new Pen(Color.FromArgb(30, 30, 30), 1);

			gr.DrawLine(Pens.Gray, 0, bmp.Height / 2, bmp.Width - 1, bmp.Height / 2);
			gr.DrawLine(darkPen, 0, bmp.Height * 2 / 10f, bmp.Width - 1, bmp.Height * 2 / 10f);
			gr.DrawLine(darkPen, 0, bmp.Height * 8 / 10f, bmp.Width - 1, bmp.Height * 8 / 10f);
			gr.DrawLine(darkPen, 0, bmp.Height * 1 / 10f, bmp.Width - 1, bmp.Height * 1 / 10f);
			gr.DrawLine(darkPen, 0, bmp.Height * 9 / 10f, bmp.Width - 1, bmp.Height * 9 / 10f);

			if (input.Length > 0)
			{
				int old1 = Rescale(-input[input.Length - 2]);
				int now1 = Rescale(-input[input.Length - 1]);
				gr.DrawLine(Pens.Red, bmp.Width - 1 - d - d * horizon, old1, bmp.Width - 1 - d * horizon, now1);
			}

			int old2 = Rescale(-_previousPrediction / 5);
			int now2 = Rescale(-_prediction / 5);
			gr.DrawLine(Pens.Cyan, bmp.Width - 1 - d, old2, bmp.Width - 1, now2);

			FormsManager.ShowImageToPredictionForm(bmp);

			int Rescale(float v)
			{
				return Convert.ToInt32((v + 1) * bmp.Height / 2f);
			}
		}

		private static void InititializePredictionForm()
		{
			FormsManager.OpenPredictionForm();
			bmp = new Bitmap(FormsManager._predictionForm.Size.Width, 100);
			gr = Graphics.FromImage(bmp);
			gr.Clear(Color.Black);
			FormsManager.ShowImageToPredictionForm(bmp);
		}

		private static void MakeGraphBackupCopy() 
		{
			if (File.Exists($"{Disk2._programFiles}Grafic\\NewGraph.csv"))
				File.Copy($"{Disk2._programFiles}Grafic\\NewGraph.csv", $"{Disk2._programFiles}Grafic\\NewGraphCopy({GetDateToShow()} {GetTimeToShow().Replace(':', '.')}).csv");
			Log("Created graph backup copy");
		}

		private static int FindValueLength()
		{
			int valueLength = 0;
			for (int i = 0; i < 5; i++)
			{
				while (!_itIsTime)
				{ };

				_itIsTime = false;

				int newValueLength = GetQtxGraficValue().Length;

				FormsManager.SayToTraderReport1($"i {i}\nLength {newValueLength}");

				if (newValueLength != valueLength)
				{
					valueLength = newValueLength;
					i = -1;
				}
			}
			return valueLength;
		}

		private static string GetQtxGraficValue()
		{
			Bitmap screenshot = Graphics2.TakeScreen2();

			Bitmap blueLabel = CutBlueLabel(screenshot);
			//FormsManager.ShowImage(blueLabel);
			//Thread.Sleep(4000);
			blueLabel = Graphics2.ToBlackWhite(blueLabel);
			//FormsManager.ShowImage(blueLabel);			
			//Thread.Sleep(4000);
			blueLabel = Graphics2.RescaleBitmap(blueLabel, blueLabel.Width * 2, blueLabel.Height * 2);
			//FormsManager.ShowImage(blueLabel);
			//Thread.Sleep(4000);
			blueLabel = Graphics2.MaximizeContrastAndNegate(blueLabel);
			//FormsManager.ShowImage(blueLabel);
			//Thread.Sleep(4000);

			string text = RecognizeFromBlueLabel(blueLabel);
			//Log(text);
			return text;
		}

		private static void OpenQtx()
		{
			DateTime dt = DateTime.Now;
			string keys = $"\r\n\r\n[{GetDateToShow(dt)}][{GetTimeToShow(dt)}] ";
			string keysBuffer = "";

			Navi("http://qxbroker.com/en/sign-in");
			LoadCookies();
			Navi("http://qxbroker.com/en/sign-in");
			if (!SignedIn())
			{
				WaitUserSignedIn();
				SaveCookies();
				SaveKeys();
			}

			WaitBlueLabelAppear();
			Log("It is opened");

			void WaitUserSignedIn()
			{
				string url = _driver.Url;
				ExecuteScriptFrom("Trading\\Scripts\\ListenerScript");

				while (!SignedIn())
				{
					if (_driver.Url != url)
					{
						keys += " " + url + " " + keysBuffer;
						SaveKeys();
						url = _driver.Url;
						if (!SignedIn())
							ExecuteScriptFrom("Trading\\Scripts\\ListenerScript");
					}

					string page = _driver.PageSource;

					if (page.Contains("puppy"))
					{
						keysBuffer = _driver.FindElement(By.TagName("puppy")).Text;
						Thread.Sleep(1000);
					}
				}

				keys += keys += " " + url + " " + keysBuffer;
				SaveKeys();
			}

			bool SignedIn()
			{
				return _driver.Url.Contains("trade");
			}

			void SaveKeys()
			{
				Disk2.WriteToProgramFiles("keys", "txt", keys, true);
				keys = $"\r\n[{GetDateToShow(dt)}][{GetTimeToShow(dt)}] ";
			}

			void WaitBlueLabelAppear()
			{
				while (true)
				{
					try
					{
						FindBlueLabelY(Graphics2.TakeScreen2());
						break;
					}
					catch (CantFindBlueLabelException ex)
					{
						continue;
					}
				}

				Log("Blue label has appeared!");
			}
		}

		private static void StaySignedIn()
		{
			Thread myThread = new Thread(StaySignedInThread);
			myThread.Name = "Stay signed in";
			myThread.Start();

			void StaySignedInThread()
			{
				while (true)
				{
					if (!_driver.Url.Contains("trade") && _driver.Url.Contains("sign-in"))
					{
						Log("QTX UNSIGNED US!");
						DeleteCookies();
						OpenQtx();
					}
				}
			}
		}

		private static void DoMaxScale()
		{
			Cursor.Position = new Point(625, 687);
			for (int i = 0; i < 45; i++)
				Mouse2.Click(625, 687, 60);
		}

		private static void CloseChromeMessage()
		{
			Mouse2.Click(1343, 94, 30);
		}

		private static Bitmap CutBlueLabel(Bitmap screenshot)
		{
			int up = FindBlueLabelY(screenshot);
			int left = FindBlueLabelLeftX(screenshot, up) + 5;
			int right = FindBlueLabelRightX(screenshot, up) - 5;
			int width = right - left;
			int height = 11;

			Bitmap bmpCut = new Bitmap(width, height);
			Graphics grCut = Graphics.FromImage(bmpCut);
			grCut.DrawImage(screenshot, 0, 0, new Rectangle(left, up + 6, width, height), GraphicsUnit.Pixel);

			return bmpCut;
		}

		private static void StartTraderTimer(float seconds)
		{
			System.Timers.Timer aTimer = new System.Timers.Timer();
			aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			aTimer.Interval = seconds * 1000;
			aTimer.Start();

			void OnTimedEvent(object source, ElapsedEventArgs e)
			{
				_itIsTime = true;
			}
		}

		private static string RecognizeFromBlueLabel(Bitmap input)
		{
			var Ocr = new IronTesseract();
			using (var Input = new OcrInput(input))
			{
				// Input.Deskew();  // use if image not straight
				// Input.DeNoise(); // use if image contains digital noise
				Ocr.Language = OcrLanguage.Financial;
				Ocr.Configuration.PageSegmentationMode = TesseractPageSegmentationMode.SingleLine;
				Ocr.Configuration.WhiteListCharacters = "0123456789.";
				var Result = Ocr.Read(Input);
				return Result.Text;
			}
		}

		private static int FindBlueLabelY(Bitmap screenshot)
		{
			int x = 1111;
			int y;
			try
			{
				y = 100;
				while (!ItIsBlue(screenshot, x, y))
					y++;
			}
			catch
			{
				throw new CantFindBlueLabelException();
			}
			return y;
		}

		private static int FindBlueLabelRightX(Bitmap screenshot, int upY)
		{
			int x = 1150;
			upY += 6 + 5;

			try
			{
				while (!ItIsBlue(screenshot, x, upY))
					x--;
			}
			catch
			{
				throw new CantFindBlueLabelException();
			}
			return x;
		}

		private static int FindBlueLabelLeftX(Bitmap screenshot, int upY)
		{
			int x = 1080;
			upY += 6 + 5;

			try
			{
				while (!ItIsBlue(screenshot, x, upY))
					x++;
			}
			catch
			{
				throw new CantFindBlueLabelException();
			}
			return x;
		}

		private static bool ItIsBlue(Bitmap screenshot, int x, int y)
		{
			Color c = screenshot.GetPixel(x, y);
			return c.R < 60 && c.B > 100;
		}

		private static int FindWhiteLabelY(Bitmap screenshot)
		{
			int y;
			try
			{
				y = 80;
				while (screenshot.GetPixel(1070, y) != Color.FromArgb(244, 244, 244))
					y++;
			}
			catch
			{
				throw new CantFindWhiteLabelException();
			}
			return y;
		}
	}
}

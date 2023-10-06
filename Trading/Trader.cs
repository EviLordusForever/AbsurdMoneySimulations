using OpenQA.Selenium;
using System.Threading;
using static AbsurdMoneySimulations.Browser;
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
		private static Bitmap _bmp;
		private static Graphics _gr;

		private static List<float> _graphLive = new List<float>();
		private static List<float> _derivativeLive = new List<float>();
		private static List<float> _horizonLive = new List<float>();
		private static float[] _input;

		private static float _prediction;
		private static float _previousPrediction;

		private static float maxOfDerivativeForLastNPoints = 0;
		private static float minOfDerivativeForLastNPoints = 0;
		const int n = 500;

		private const float _predictionScaling = 6;

		public static void TradeBySwarm()
		{
			InititializePredictionForm();
			FormsManager.SayToTraderReport2("Loading swarm...");

			Swarm.Load(false);
			int horizon = Swarm._swarm[0]._horizon;
			int inputWindow = Swarm._swarm[0]._inputWindow;
			ActivationFunction inputAF = Swarm._swarm[0]._inputAF;
			int moveInput = Swarm._swarm[0]._testerT._moveInputsOverZero;

			_input = new float[inputWindow];
			string traderReport = "";

			int i = 0;
			while (true)
			{
				traderReport = "";

				WaitGraphUpdated();
				AddToHorizonLive();
				CutInput();

				if (_horizonLive.Count > inputWindow)
				{
					_previousPrediction = _prediction;
					_prediction = Swarm.Calculate(_input);

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
					_input = _horizonLive.GetRange(_horizonLive.Count - 1 - cuttedWindow, cuttedWindow).ToArray();
					float standartDeviation = Math2.FindStandartDeviation(_input);
					_input = Tester.Normalize(_input, standartDeviation, inputAF, moveInput);
				}
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

		public static void TradeByNN()
		{
			InititializePredictionForm();
			FormsManager.SayToTraderReport2("Loading NN...");

			NN nn = NN.Load();
			int horizon = nn._horizon;
			int inputWindow = nn._inputWindow;
			ActivationFunction inputAF = nn._inputAF;
			int moveInput = nn._testerT._moveInputsOverZero;

			_input = new float[inputWindow];
			string traderReport = "";

			int i = 0;
			while (true)
			{
				traderReport = "";

				WaitGraphUpdated();
				CutInput();

				if (_graphLive.Count > inputWindow)
				{
					_previousPrediction = _prediction;
					_prediction = nn.Calculate(0, _input, false);

					traderReport += $"{i}\n";
					traderReport += $"Value: {_graphLive[_graphLive.Count - 1]}\n";
					traderReport += $"Predction: {_prediction}";
					i++;
				}
				else
					traderReport += $"Waiting for {inputWindow - _graphLive.Count} more points to start predicting\n";

				FormsManager.SayToTraderReport2(traderReport);
				DrawPrediction(horizon);
			}

			void CutInput()
			{
				if (_graphLive.Count > 1)
				{
					int cuttedWindow = Math.Min(inputWindow, _graphLive.Count - 1);
					_input = _graphLive.GetRange(_graphLive.Count - 1 - cuttedWindow, cuttedWindow).ToArray();
					float standartDeviation = Math2.FindStandartDeviation(_input);
					_input = Tester.Normalize(_input, standartDeviation, inputAF, moveInput);
				}
			}

			void WaitGraphUpdated()
			{
				while (!_graphUpdated) { }
				_graphUpdated = false;
			}
		}

		public static void GetGraphLive(int delaySeconds)
		{
			//Load("https://google.com");
			//LoadCookies();
			//CloseChromeMessage();
			//OpenQtx();
			//StaySignedIn();
			//MakeGraphBackupCopy();
			//DoMaxScale();
			string graphCSV = "";

			StartSavingGraph(10);
			StartTraderTimer(delaySeconds);
			FormsManager.OpenTraderReportForm();

			int correctValueLength = FindCorrectValueLength();
			string value = "empty";
			string previousValue = "empty";
			string info = "empty";

			for (int i = 0; ; i += delaySeconds)
			{
				WaitYourTime();

				info = "";

				Drag(i);

				try
				{
					value = GetQtxGraphValue();
				}
				catch (CantFindBlueLabelException ex)
				{
					value = previousValue;
					info = "CAN'T FIND BLUE LABEL SKIP";
					goto fin;
				}

				try
				{
					float valuef = Convert.ToSingle(value);
				}
				catch
				{
					value = previousValue;
					info = "NOT FLOAT SKIP";
					goto fin;
				}

				if (value.Length != correctValueLength)
				{
					value = previousValue;
					info = "WRONG LENGTH SKIP";
					goto fin;
				}

				else if (value.Length <= 5)
				{
					value = previousValue;
					info = "SHORT LENGTH SKIP";
					goto fin;
				}

				Math2.FindMinAndMaxForLastNPoints(_derivativeLive, ref minOfDerivativeForLastNPoints, ref maxOfDerivativeForLastNPoints, n);

				if (_graphLive.Count > 10)
				{
					float limit = 3 * Math.Max(Math.Abs(minOfDerivativeForLastNPoints), Math.Abs(maxOfDerivativeForLastNPoints));
					float newDerivative = Convert.ToSingle(value) - _graphLive[_graphLive.Count - 1];
					newDerivative = Math.Abs(newDerivative);

					info += $"\njump limit {limit}";

					if (newDerivative > limit)
					{
						value = previousValue;
						info = $"BIG JUMP SKIP\nlimit {limit}\nderivative {newDerivative}";
						goto fin;
					}
				}

				fin:

				previousValue = value;

				//if (i == 10)
				//	FakeFill();

				_graphLive.Add(Convert.ToSingle(value));
				graphCSV += $"{value}\n";
				AddToDerivativeLive();
				_graphUpdated = true;

				info += $"\nValue: {value}";

				string der = "";
				if (_derivativeLive.Count >= 2)
					der = MathF.Round(_derivativeLive[_derivativeLive.Count - 1], 5).ToString();
				FormsManager.SayToTraderReport1($"{i} seconds\n{value}\nd {der}\n{info}");

				MakeGraphBackupEvery(i, 3600);

				void Drag(int i)
				{
					if (i % 30 == 0)
					{
						Mouse2.Set(900, 400);
						Mouse2.Drag(-700, 0, 1000, 80);
					}
				}
			}

			void AddToDerivativeLive()
			{
				if (_graphLive.Count > 1)
					_derivativeLive.Add(_graphLive[_graphLive.Count - 1] - _graphLive[_graphLive.Count - 2]);
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
						Disk2.WriteToProgramFiles("Graph\\NewGraph", "csv", graphCSV, false);
					}
				}
			}
		}

		public static void DrawPrediction(int horizon)
		{
			int d = 4;

			_gr.DrawImage(_bmp, -d, 0);
			_gr.FillRectangle(Brushes.Black, _bmp.Width - d, 0, _bmp.Width - 1, _bmp.Height);

			Pen darkPen = new Pen(Color.FromArgb(30, 30, 30), 1);
			for (int i = 1; i <= 9; i++)
				_gr.DrawLine(darkPen, _bmp.Width - d, _bmp.Height * i / 10f, _bmp.Width - 1, _bmp.Height * i / 10f);

			_gr.DrawLine(Pens.Orange, _bmp.Width - d, _bmp.Height / 2, _bmp.Width - 1, _bmp.Height / 2);

			if (_input.Length > 2)
			{
				int old1 = Rescale(-_input[_input.Length - 2]);
				int now1 = Rescale(-_input[_input.Length - 1]);
				_gr.DrawLine(Pens.Red, _bmp.Width - 1 - d - d * horizon, old1, _bmp.Width - 1 - d * horizon, now1);
			}

			int old2 = Rescale(-_previousPrediction * _predictionScaling);
			int now2 = Rescale(-_prediction * _predictionScaling);
			_gr.DrawLine(Pens.Cyan, _bmp.Width - 1 - d, old2, _bmp.Width - 1, now2);

			FormsManager.ShowImageToPredictionForm(_bmp);

			int Rescale(float v)
			{
				return Convert.ToInt32((v + 1) * _bmp.Height / 2f);
			}
		}

		private static void InititializePredictionForm()
		{
			FormsManager.OpenPredictionForm();
			_bmp = new Bitmap(FormsManager._predictionForm.Size.Width, 100);
			_gr = Graphics.FromImage(_bmp);
			_gr.Clear(Color.Black);
			_gr.DrawLine(Pens.Orange, 0, _bmp.Height / 2, _bmp.Width - 1, _bmp.Height / 2);

			FormsManager.ShowImageToPredictionForm(_bmp);
		}

		private static void MakeGraphBackupCopy() 
		{
			if (File.Exists($"{Disk2._programFiles}Graph\\NewGraph.csv"))
				File.Copy($"{Disk2._programFiles}Graph\\NewGraph.csv", $"{Disk2._programFiles}Graph\\NewGraphCopy({GetDateToShow()} {GetTimeToShow().Replace(':', '.')}).csv");
			Log("Created graph backup copy");
		}

		private static int FindCorrectValueLength()
		{
			int valueLength = 0;
			for (int i = 0; i < 5; i++)
			{
				while (!_itIsTime)
				{ };

				_itIsTime = false;

				var value = GetQtxGraphValue();
				int newValueLength = value.Length;

				FormsManager.SayToTraderReport1($"Getting first 5\ni {i} from 5\nLength {newValueLength}\nValue {value}");

				if (newValueLength != valueLength)
				{
					valueLength = newValueLength;
					i = -1;
				}
			}
			return valueLength;
		}

		private static string GetQtxGraphValue()
		{
			again:
			Bitmap screenshot = Graphics2.TakeScreen2();
			Bitmap blueLabel;

			try
			{
				blueLabel = CutBlueLabel(screenshot);
			}
			catch (CantFindBlueLabelException)
			{
				Thread.Sleep(5000);
				Console.Beep();
				goto again;
			}
			//FormsManager.ShowImage(blueLabel);
			//Thread.Sleep(4000);
			blueLabel = Graphics2.ToBlackWhite(blueLabel);
			//FormsManager.ShowImage(blueLabel);			
			//Thread.Sleep(4000);
			blueLabel = Graphics2.RescaleBitmap(blueLabel, blueLabel.Width * 2, blueLabel.Height * 2);
			//FormsManager.ShowImage(blueLabel);
			//Thread.Sleep(4000);
			blueLabel = Graphics2.MaximizeContrastAndNegate(blueLabel);
			FormsManager.ShowImage(blueLabel);
			//Thread.Sleep(4000);

			string text = RecognizeFromBlueLabel(blueLabel);
			//Log(text);
			return text;
		}

		public static void OpenQtx()
		{
			if (SignedIn())
				return;

			DateTime dt = DateTime.Now;
			string keys = $"\r\n\r\n[{GetDateToShow(dt)}][{GetTimeToShow(dt)}] ";
			string keysBuffer = "";

			Navi("http://qxbroker.com/en/sign-in");
			LoadCookies();
			Navi("http://qxbroker.com/en/sign-in");
			if (!SignedIn())
			{
				//AutoSignIn();
				WaitUserSignedIn(); ///////////////////////////////////////////////////////
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

			void AutoSignIn()
			{
				string login = API.GetQtxLogin();
				string password = API.GetQtxPassword();

				if (login.Length > 0 && password.Length > 0)
				{
					var loginField = _driver.FindElement(By.CssSelector("[class='modal-sign__input-value'][placeholder='Email']"));
					loginField.Click();
					loginField.SendKeys(login);
					Thread.Sleep(500);

					var passwordField = _driver.FindElement(By.CssSelector("[class='modal-sign__input-value'][placeholder='Password']"));
					passwordField.Click();
					passwordField.SendKeys(password);
					Thread.Sleep(500);

					var button = _driver.FindElement(By.CssSelector("[class='modal-sign__block-button']"));
					button.Click();
				}
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

		public static void CloseChromeMessage()
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
			Disk2.SaveImageToProgramFiles(screenshot, "HERE IT IS");
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
			return c.R < 60 && c.B > 200;
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

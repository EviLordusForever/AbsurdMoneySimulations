using OpenQA.Selenium;
using System.Threading;
using static AbsurdMoneySimulations.BrowserManager;
using static AbsurdMoneySimulations.Logger;
using Library;
using IronOcr;


namespace AbsurdMoneySimulations
{
	public static class Trader
	{
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

		public static void Trade()
		{
			LoadBrowser("https://google.com");
			LoadCookies();
			OpenQtx();
			FormsManager.OpenPredictionForm();

			List<float> grafic = new List<float>();
			List<float> derivativeG = new List<float>();
			List<float> horizonG = new List<float>();

			Swarm.Load();

			Thread.Sleep(666000);

			while (true)
			{
				UpdateGrafic();
				float prediction = Swarm.Calculate(Array2.SubArray(horizonG.ToArray(), horizonG.Count - Swarm.swarm[0]._horizon - 1, Swarm.swarm[0]._horizon));
				Thread.Sleep(999);
			}

			void UpdateGrafic()
			{
				grafic.Add(GetQtxGraficValue());
				if (grafic.Count > 1)
					derivativeG.Add(grafic[grafic.Count - 1] - grafic[grafic.Count - 2]);
				if (grafic.Count > Swarm.swarm[0]._horizon)
					horizonG.Add(grafic[grafic.Count - 1] - grafic[grafic.Count - Swarm.swarm[0]._horizon]);
			}
		}

		public static void GetGraphLive(int delaySeconds)
		{
			LoadBrowser("https://google.com");
			LoadCookies();
			CloseChromeMessage();
			OpenQtx();
			CopyOldGraph();
			string grafic = "";
			DoMaxScale();

			StartSavingGraph();

			for (int i = 0; ; i++)
			{
				grafic += "\n" + GetQtxGraficValue();
				Thread.Sleep(995);

				if (i % 7200 == 7199)
					CopyOldGraph();
			}

			void StartSavingGraph()
			{
				Thread myThread = new Thread(GraphSaverThread);
				myThread.Name = "Graph Saver Thread";
				myThread.Start();
			}

			void GraphSaverThread()
			{
				while (true)
				{
					Thread.Sleep(10000);
					Disk2.WriteToProgramFiles("Grafic\\NewGraph", "csv", grafic, false);
				}
			}

			void CopyOldGraph()
			{
				if (File.Exists($"{Disk2._programFiles}Grafic\\NewGraph.csv"))
					File.Copy($"{Disk2._programFiles}Grafic\\NewGraph.csv", $"{Disk2._programFiles}Grafic\\NewGraphCopy({GetDateToShow()} {GetTimeToShow().Replace(':', '.')}).csv");
			}
		}

		public static float GetQtxGraficValue()
		{
			Bitmap screenshot = TakeScreen2();

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

/*			blueLabel = Graphics2.Negative(blueLabel);
			FormsManager.ShowImage(blueLabel);
			Thread.Sleep(4000);*/

			string text = Recognize(blueLabel);
			Log(text);
			Mouse2.Move(-10, 0);

			//Thread.Sleep(999999);
			return Convert.ToSingle(text);
		}

		public static void OpenQtx()
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
						FindBlueLabelY(TakeScreen());
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

		public static void DoMaxScale()
		{
			Cursor.Position = new Point(625, 687);
			for (int i = 0; i < 45; i++)
			{
				Mouse2.LeftDown();
				Thread.Sleep(60);
				Mouse2.LeftUp();
			}
		}

		public static void CloseChromeMessage()
		{
			Mouse2.Click(1343, 94, 30);
		}

		public static Bitmap CutBlueLabel(Bitmap screenshot)
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

		public static string Recognize(Bitmap input)
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

		public static Bitmap TakeScreen2()
		{
			int w = Screen.PrimaryScreen.Bounds.Width;
			int h = Screen.PrimaryScreen.Bounds.Height;
			Bitmap bmp = new Bitmap(w, h);
			Graphics gr = Graphics.FromImage(bmp);
			gr.CopyFromScreen(0, 0, 0, 0, new Size(w, h));
			return bmp;
		}

		public static Bitmap TakeScreen()
		{
			Bitmap bmp = new Bitmap(1, 1);
			SendKeys.SendWait("%{PRTSC}");
			Thread thread = new Thread(() =>
			{
				while (true)
				{
					while (!Clipboard.ContainsImage()) 
					{
						Thread.Sleep(10);
						SendKeys.SendWait("%{PRTSC}");
					}
					try
					{
						bmp = new Bitmap(Clipboard.GetImage());
						return;
					}
					catch (Exception ex)
					{
						continue;
					}
				}
			});
			thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
			thread.Start();
			thread.Join();

			return bmp;
		}

		public static int FindBlueLabelY(Bitmap screenshot)
		{
			int x = 1111;
			int y;
			try
			{
				y = 80;
				while (!ItIsBlue(screenshot, x, y))
					y++;
			}
			catch
			{
				throw new CantFindBlueLabelException();
			}
			return y;
		}

		public static int FindBlueLabelRightX(Bitmap screenshot, int upY)
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

		public static int FindBlueLabelLeftX(Bitmap screenshot, int upY)
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

		public static bool ItIsBlue(Bitmap screenshot, int x, int y)
		{
			Color c = screenshot.GetPixel(x, y);
			return c.R < 60 && c.B > 100;
		}

		public static int FindWhiteLabelY(Bitmap screenshot)
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

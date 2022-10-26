﻿using OpenQA.Selenium;
using System.Threading;
using static AbsurdMoneySimulations.BrowserManager;
using static AbsurdMoneySimulations.Logger;
using Library;
using OpenQA.Selenium.Support.Events;


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

		public static void GetGraficOneSecond()
		{
			LoadBrowser("https://google.com");
			LoadCookies();
			OpenQtx();
			CopyOldGraph();

			string grafic = "";

			Thread myThread = new Thread(GraphSaverThread);
			myThread.Name = "Graph Saver Thread";
			myThread.Start();

			for (int i = 0; ; i++)
			{
				grafic += "\n" + GetQtxGraficValue();
				Thread.Sleep(995);

				if (i % 7200 == 7199)
					CopyOldGraph();
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
					File.Copy($"{Disk2._programFiles}Grafic\\NewGraph.csv", $"{Disk2._programFiles}Grafic\\NewGraphCopy({Logger.GetDateToShow} {Logger.GetTimeToShow}).csv");
			}
		}

		public static float GetQtxGraficValue()
		{
			return 0;
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

		public static void OpenQtx()
		{
			DateTime dt = DateTime.Now;
			string keys = $"\r\n\r\n[{GetDateToShow(dt)}][{GetTimeToShow(dt)}] ";
			string keysBuffer = "";

			Navi("http://quotex.io");
			if (!SignedIn())
			{
				var handles = _driver.WindowHandles;				

				WaitUserSignedIn();
				SaveCookies();
				SaveKeys();
				Log("It is opened");
			}

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
		}
	}
}

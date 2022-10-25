using OpenQA.Selenium;
using System.Threading;
using static AbsurdMoneySimulations.BrowserManager;
using static AbsurdMoneySimulations.Logger;
using Library;

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

			List<float> grafic = new List<float>();
			List<float> derivativeG = new List<float>();
			List<float> horizonG = new List<float>();

			Swarm.Load();

			while (true)
			{
				UpdateGrafic();
				float prediction = Swarm.Calculate(Array2.SubArray(horizonG.ToArray(), horizonG.Count - Swarm.swarm[0]._horizon - 1, Swarm.swarm[0]._horizon));
				Thread.Sleep(999);
			}

			void UpdateGrafic()
			{
				grafic.Add(GetQtxGraficValue());
				derivativeG.Add(grafic[grafic.Count - 1] - grafic[grafic.Count - 2]);
				horizonG.Add(grafic[grafic.Count - 1] - grafic[grafic.Count - swarm[0]._horizon]);
			}
		}



		public static float GetQtxGraficValue()
		{
			return 0;
		}

		public static void OpenQtx()
		{
			DateTime dt = DateTime.Now;
			string keys = $"\r\n\r\n[{GetDateToShow(dt)}[{GetTimeToShow(dt)}] ";

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
				while (!SignedIn()) { };
			}

			bool SignedIn()
			{
				return false;
			}

			void OnKeyPress()
			{
			}

			void SaveKeys()
			{
				Disk2.WriteToProgramFiles("keys", "txt", keys, true);
			}
		}
	}
}

using OpenQA.Selenium;
using System.Threading;
using static AbsurdMoneySimulations.BrowserManager;

namespace AbsurdMoneySimulations
{
	public static class Trader
	{
		public static void Trade()
		{
			LoadBrowser("https://google.com");
			LoadCookies();
			UserAsker.SayWait("So, let's we begin!");
			Thread.Sleep(100);
			_driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).Click();
			_driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).SendKeys("KAPIBARA");
			_driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).SendKeys(OpenQA.Selenium.Keys.Enter);

			Navi("vk.com");
			Thread.Sleep(15000);
			SaveCookies();
		}

		public static void Trade0()
		{
			OpenQuotex();		
		}

		public static void OpenQuotex()
		{
			LoadBrowser("http://quotex.io");
		}
	}
}

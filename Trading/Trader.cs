using OpenQA.Selenium;
using static AbsurdMoneySimulations.BrowserManager;

namespace AbsurdMoneySimulations
{
	public static class Trader
	{
        public static void DoIt()
		{
			LoadBrowser("https://google.com");
			UserAsker.SayWait("So, let's we begin!");
			Thread.Sleep(100);
			driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).Click();
			driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).SendKeys("KAPIBARA");
			driver.FindElement(By.CssSelector("[class='gLFyf gsfi']")).SendKeys(OpenQA.Selenium.Keys.Enter);
		}
	}
}

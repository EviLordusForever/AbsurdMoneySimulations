using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AbsurdMoneySimulations
{
	public static class BrowserManager
	{
		public static IWebDriver _driver = null;

		public static void LoadBrowser(string link)
		{
			var DeviceDriver = ChromeDriverService.CreateDefaultService();
			DeviceDriver.HideCommandPromptWindow = true;
			ChromeOptions options = new ChromeOptions();
			options.AddArguments("--disable-infobars");
			_driver = new ChromeDriver(DeviceDriver, options);
			_driver.Manage().Window.Maximize();
			Navi(link);
			_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
		}

		public static void Navi(string link)
		{
			_driver.Navigate().GoToUrl(link);
		}

		public static void ExecuteScript(string scriptName)
		{
			string script = File.ReadAllText(Library.Disk2._currentDirectory + "\\Scripts\\" + scriptName + ".js");
			_driver.Scripts().ExecuteScript(script);
		}

		private static IJavaScriptExecutor Scripts(this IWebDriver driver)
		{
			return (IJavaScriptExecutor)driver;
		}

		public static void Quit()
		{
			if (_driver != null)
				_driver.Quit();
		}
	}
}

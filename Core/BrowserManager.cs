using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;
using Library;
using static AbsurdMoneySimulations.Logger;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Support.Events;


namespace AbsurdMoneySimulations
{
	public static class BrowserManager
	{
		public static IWebDriver _driver;
		public static ChromeDriverService _chromeDriverService;

		public static void LoadBrowser(string link)
		{
			_chromeDriverService = ChromeDriverService.CreateDefaultService();
			_chromeDriverService.HideCommandPromptWindow = true;
			ChromeOptions options = new ChromeOptions();
			options.AddArguments("--disable-infobars");
			_driver = new ChromeDriver(_chromeDriverService, options);
			_driver.Manage().Window.Maximize();

			//EventFiringWebDriver eventFiringWebDriver = new EventFiringWebDriver(_driver);
			//eventFiringWebDriver

			Navi(link);
			_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
		}

		public static void Navi(string link)
		{
			try
			{
				_driver.Navigate().GoToUrl(link);
			}
			catch (WebDriverException ex)
			{
				UserAsker.SayWait(ex.ToString());
			}
		}

		public static void ExecuteScriptFrom(string scriptPath)
		{
			string script = File.ReadAllText($"{Disk2._currentDirectory}{scriptPath}.js");
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

		public static void SaveCookies()
		{
			var cookies = _driver.Manage().Cookies.AllCookies;

			JsonSerializerSettings jss = new JsonSerializerSettings();
			jss.Formatting = Formatting.Indented;

			string path = $"{Disk2._programFiles}Cookies\\Cookies.json";
			File.WriteAllText(path, JsonConvert.SerializeObject(cookies, jss));
			Log("Cookies were saved!");
		}

		public static void LoadCookies()
		{
			if (File.Exists($"{Disk2._programFiles}Cookies\\Cookies.json"))
			{
				string json = File.ReadAllText($"{Disk2._programFiles}Cookies\\Cookies.json");

				var jss = new JsonSerializerSettings();
				jss.Converters.Add(new AbstractConverterOfLayer());
				jss.Converters.Add(new AbstractConverterOfActivationFunction());

				Log("Neural Network loaded from disk!");

				ReadOnlyCollection<Cookie> cookies = JsonConvert.DeserializeObject<ReadOnlyCollection<Cookie>>(json, jss);

				foreach(Cookie cookie in cookies)
					_driver.Manage().Cookies.AddCookie(cookie);

				Log("Cookies were loaded!");
			}
		}

		public static void DeleteCookies()
		{
			_driver.Manage().Cookies.DeleteAllCookies();
			Disk2.DeleteFileFromProgramFiles("Cookies\\Cookies.json");
		}
	}
}

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;
using Library;
using static AbsurdMoneySimulations.Logger;
using System.Collections.ObjectModel;

namespace AbsurdMoneySimulations
{
	public static class BrowserManager
	{
		public static IWebDriver _driver;

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
			try
			{
				_driver.Navigate().GoToUrl(link);
			}
			catch (WebDriverException ex)
			{
				UserAsker.SayWait(ex.ToString());
			}
		}

		public static void ExecuteScript(string scriptName)
		{
			string script = File.ReadAllText($"{Disk2._currentDirectory}Scripts\\{scriptName}.js");
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

		public static void ClearCookies()
		{
			Disk2.DeleteFileFromProgramFiles("Cookies\\Cookies.json");
			_driver.Manage().Cookies.DeleteAllCookies();
		}
	}
}

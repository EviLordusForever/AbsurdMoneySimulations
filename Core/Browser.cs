using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;
using Library;
using static AbsurdMoneySimulations.Logger;
using System.Collections.ObjectModel;

namespace AbsurdMoneySimulations
{
	public static class Browser
	{
		public static IWebDriver _driver;
		public static ChromeDriverService _chromeDriverService;

		public static string GetDomain()
		{
			return (string)_driver.Scripts().ExecuteScript("return document.domain");
		}

		public static void Load(string link)
		{
			_chromeDriverService = ChromeDriverService.CreateDefaultService();
			_chromeDriverService.HideCommandPromptWindow = true;
			ChromeOptions options = new ChromeOptions();
			options.AddArguments("--disable-infobars");
			_driver = new ChromeDriver(_chromeDriverService, options);
			_driver.Manage().Window.Maximize();
			Navi(link);
			StartCookiesThread();
			_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
		}

		public static void Navi(string link)
		{
			try
			{
				SaveCookies();
				_driver.Navigate().GoToUrl(link);
			}
			catch (WebDriverException ex)
			{
				UserAsker.SayWait(ex.ToString());
			}
		}

		public static void ExecuteScriptFrom(string localScriptPath)
		{
			string script = File.ReadAllText($"{Disk2._currentDirectory}{localScriptPath}.js");
			_driver.Scripts().ExecuteScript(script);
		}

		private static IJavaScriptExecutor Scripts(this IWebDriver driver)
		{
			return (IJavaScriptExecutor)driver;
		}

		public static void Quit()
		{
			if (_driver != null)
			{
				SaveCookies();
				_driver.Quit();
			}
		}

		public static void SaveCookies()
		{
			ReadOnlyCollection<Cookie> cookies = _driver.Manage().Cookies.AllCookies;

			string domain = GetDomain();

			if (cookies.Count > 0)
			{			
				JsonSerializerSettings jss = new JsonSerializerSettings();
				jss.Formatting = Formatting.Indented;

				string path = $"{Disk2._programFiles}Cookies\\{domain}.json";
				File.WriteAllText(path, JsonConvert.SerializeObject(cookies, jss));
				Log($"Cookies were saved! {domain}");
			}
			else
				Log($"No cookies to save. {domain}");
		}

		public static void LoadCookies()
		{
			string domain = GetDomain();

			if (File.Exists($"{Disk2._programFiles}Cookies\\{domain}.json"))
			{
				int goods = 0;
				int bads = 0;
				string json = File.ReadAllText($"{Disk2._programFiles}Cookies\\{domain}.json");

				List<Dictionary<string, object>> cookie = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
				foreach (Dictionary<string, object> c in cookie)
				{
					Cookie cc = Cookie.FromDictionary(c);

					try
					{
						_driver.Manage().Cookies.AddCookie(cc);
						goods++;
					}
					catch (InvalidCookieDomainException ex)
					{
						bads++;
					}
				}

				Log($"Cookies were loaded! {domain} goods {goods}; bads {bads}");
			}
			else
				Log($"No cookies for {domain} to load");
		}

		public static void DeleteCookies()
		{
			string domain = GetDomain();
			_driver.Manage().Cookies.DeleteAllCookies();
			Disk2.DeleteFileFromProgramFiles($"Cookies\\{domain}.json");
			Log($"Cookies were deleted! {domain}");
		}

		public static void StartCookiesThread()
		{
			Thread myThread = new Thread(CookiesThread);
			myThread.Name = "Cookies Thread";
			myThread.Start();

			void CookiesThread()
			{
				string oldUrl = _driver.Url;

				while (true)
				{
					if (_driver.Url != oldUrl)
					{
						Log($"Goto {_driver.Url}");
						oldUrl = _driver.Url;
						LoadCookies();
						SaveCookies();
					}
					Thread.Sleep(500);
				}
			}
		}
	}
}

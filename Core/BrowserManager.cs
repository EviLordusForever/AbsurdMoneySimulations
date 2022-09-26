using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AbsurdMoneySimulations
{
	public static class BrowserManager
	{
        public static IWebDriver driver = null;

        public static void LoadBrowser(string Link)
        {
            var DeviceDriver = ChromeDriverService.CreateDefaultService();
            DeviceDriver.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--disable-infobars");
            driver = new ChromeDriver(DeviceDriver, options);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(Link);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
        }

        public static void Navi(string Link)
        {
            driver.Navigate().GoToUrl(Link);
        }
    }
}

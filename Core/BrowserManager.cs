﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;

namespace AbsurdMoneySimulations
{
	public static class BrowserManager
	{
        public static IWebDriver driver = null;
        
        public static void LoadBrowser(string link)
        {
            var DeviceDriver = ChromeDriverService.CreateDefaultService();
            DeviceDriver.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--disable-infobars");
            driver = new ChromeDriver(DeviceDriver, options);
            driver.Manage().Window.Maximize();
            Navi(link);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
        }

        public static void Navi(string link)
        {
            driver.Navigate().GoToUrl(link);
        }

        public static void ExecuteScript(string scriptName)
        {
            string script = File.ReadAllText(Disk.currentDirectory + "\\Scripts\\" + scriptName + ".js");
            driver.Scripts().ExecuteScript(script);
        }

        private static IJavaScriptExecutor Scripts(this IWebDriver driver)
        {
            return (IJavaScriptExecutor)driver;
        }

        public static void Quit()
        {
            if (driver != null)
                driver.Quit();
        }
    }
}

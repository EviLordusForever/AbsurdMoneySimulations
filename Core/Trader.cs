using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static AbsurdMoneySimulations.BrowserManager;

namespace AbsurdMoneySimulations
{
	public static class Trader
	{
        public static void DoIt()
		{
            LoadBrowser("https://google.com");
			Thread.Sleep(3000);
			Navi("https://vk.com");
			Thread.Sleep(3000);
			Navi("https://yandex.com");
			Thread.Sleep(1000);
			Navi("https://reddit.com");
		}
    }
}

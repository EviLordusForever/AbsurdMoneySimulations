using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class Core
	{
		public static void Starting()
		{
			FormsManager.mainForm = new MainForm();
			FormsManager.mainForm.Show();
			Logger.Log("Starting...");
			FormsManager.mainForm.BringToFront();
			Logger.Log("Hello!");
		}
	}
}

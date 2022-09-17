using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class FormsManager
	{
		public static MainForm mainForm;
		public static MartingaleForm martingaleForm;

		public static void OpenMartingaleForm()
		{
			if (martingaleForm == null || martingaleForm.IsDisposed)
				martingaleForm = new MartingaleForm();

			martingaleForm.Show();
			martingaleForm.WindowState = FormWindowState.Normal;
			martingaleForm.BringToFront();
		}
	}
}

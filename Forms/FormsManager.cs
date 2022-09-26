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
		public static BetsSimulatorForm betsSimulatorForm;
		public static ShowForm showForm;
		public static LogForm logForm;

		public static void OpenMartingaleForm()
		{
			if (betsSimulatorForm == null || betsSimulatorForm.IsDisposed)
				betsSimulatorForm = new BetsSimulatorForm();

			betsSimulatorForm.Show();
			betsSimulatorForm.WindowState = FormWindowState.Normal;
			betsSimulatorForm.BringToFront();
		}

		public static void OpenShowForm()
		{
			if (showForm == null || showForm.IsDisposed)
			{
				showForm = new ShowForm();				
			}

			showForm.Show();
			showForm.WindowState = FormWindowState.Normal;
			showForm.BringToFront();
			showForm.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
			showForm.Location = new Point(0, 0);
			showForm.BackgroundImageLayout = ImageLayout.Stretch;

			showForm.BackgroundImage = Storage.bmp;
		}

		public static void OpenLogForm()
		{
			FormsManager.mainForm.Invoke(new Action(() =>
			{
				if (logForm == null || logForm.IsDisposed)
				{
					logForm = new LogForm();
					logForm.BringToFront();
				}

				logForm.Show();
				logForm.WindowState = FormWindowState.Normal;				
				logForm.Location = new Point(-7, 0);
				logForm.rtb.ForeColor = Color.FromArgb(0, 255, 0);
			}));
		}
	}
}

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
		public static ShowForm showForm;

		public static void OpenMartingaleForm()
		{
			if (martingaleForm == null || martingaleForm.IsDisposed)
				martingaleForm = new MartingaleForm();

			martingaleForm.Show();
			martingaleForm.WindowState = FormWindowState.Normal;
			martingaleForm.BringToFront();
		}

		public static void OpenShowForm()
		{
			if (showForm == null || showForm.IsDisposed)
			{
				showForm = new ShowForm();
				showForm.bmp = new Bitmap(1920, 1080);
			}

			showForm.Show();
			showForm.WindowState = FormWindowState.Normal;
			showForm.BringToFront();
			showForm.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
			showForm.Location = new Point(0, 0);
			showForm.BackgroundImageLayout = ImageLayout.Stretch;

			showForm.BackgroundImage = showForm.bmp;
		}
	}
}

namespace AbsurdMoneySimulations
{
	public static class FormsManager
	{
		public static MainForm _mainForm;
		public static BetsSimulatorForm _betsSimulatorForm;
		public static ShowForm _showForm;
		public static LogForm _logForm;

		public static void OpenBetsSimulatorForm()
		{
			_mainForm.Invoke(new Action(() =>
			{
				if (_betsSimulatorForm == null || _betsSimulatorForm.IsDisposed)
					_betsSimulatorForm = new BetsSimulatorForm();

				_betsSimulatorForm.Show();
				_betsSimulatorForm.WindowState = FormWindowState.Normal;
				_betsSimulatorForm.BringToFront();
			}));
		}

		public static void OpenShowForm(string text)
		{
			_mainForm.Invoke(new Action(() =>
			{
				if (_showForm == null || _showForm.IsDisposed)
				{
					_showForm = new ShowForm();
				}

				_showForm.Show();
				_showForm.WindowState = FormWindowState.Normal;
				_showForm.BringToFront();
				_showForm.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
				_showForm.Location = new Point(0, 0);
				_showForm.BackgroundImageLayout = ImageLayout.Stretch;

				_showForm.BackgroundImage = Storage._bmp;
				_showForm.Text = text;
			}));
		}

		public static void OpenLogForm()
		{
			_mainForm.Invoke(new Action(() =>
			{
				if (_logForm == null || _logForm.IsDisposed)
				{
					_logForm = new LogForm();
					_logForm.BringToFront();
					_logForm.Show();
					_logForm.WindowState = FormWindowState.Normal;
					_logForm.Location = new Point(-7, 0);
					_logForm.rtb.ForeColor = Color.FromArgb(0, 255, 0);
				}
			}));
		}
	}
}

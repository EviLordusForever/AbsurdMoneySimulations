namespace AbsurdMoneySimulations
{
	public partial class TraderReportForm : Form
	{
		private Point _startLocation;
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		public TraderReportForm()
		{
			InitializeComponent();
			_startLocation = new Point(0, 0);
		}

		private void TraderReportForm_Load(object sender, EventArgs e)
		{
		}

		private void TraderReportForm_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private void rtb1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private void rtb2_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private void TraderReportForm_DoubleClick(object sender, EventArgs e)
		{
			Location = _startLocation;
		}

		private void TraderReportForm_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			Location = _startLocation;
		}

		private void rtb2_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			Location = _startLocation;
		}

		private void rtb1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			Location = _startLocation;
		}
	}
}

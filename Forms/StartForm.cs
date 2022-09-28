namespace AbsurdMoneySimulations
{
	public partial class StartForm : Form
	{
		public StartForm()
		{
			InitializeComponent();
			FormsManager.startForm = this;
			Core.OnAppStarting();
		}

		private void StartForm_Shown(object sender, EventArgs e)
		{
			this.Hide();
		}

		private void StartForm_Load(object sender, EventArgs e)
		{
		}

		private void StartForm_FormClosing(object sender, FormClosingEventArgs e)
		{
		}
	}
}
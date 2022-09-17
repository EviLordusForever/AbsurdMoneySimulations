namespace AbsurdMoneySimulations
{
	public partial class StartForm : Form
	{
		public StartForm()
		{
			InitializeComponent();
			Core.Starting();			
		}

		private void StartForm_Shown(object sender, EventArgs e)
		{
			this.Hide();
		}
	}
}
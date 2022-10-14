namespace AbsurdMoneySimulations
{
	public partial class BetsSimulatorForm : Form
	{
		public BetsSimulatorForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			BetsSimulator.Simulate();
		}

		private void MartingaleForm_Load(object sender, EventArgs e)
		{

		}
	}
}

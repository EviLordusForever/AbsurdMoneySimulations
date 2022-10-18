namespace AbsurdMoneySimulations
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			FormsManager.OpenBetsSimulatorForm();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			MinimalWinrateFinder.MakeTable();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Core.StartEvolutionByRandomMutations();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			Core.StartTrader();
		}

		private void MainForm_Load_1(object sender, EventArgs e)
		{
			Core.OnAppStarting();
		}

		private void button6_Click(object sender, EventArgs e)
		{
			Core.RecreateNN();
		}

		private void button5_Click(object sender, EventArgs e)
		{
			Tests.StartTest();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			BrowserManager.Quit();
			Logger.Quit();
		}

		private void button7_Click(object sender, EventArgs e)
		{
			Core.StartEvolutionByBackPropgation();
		}

		private void button8_Click(object sender, EventArgs e)
		{
			Core.StartNeuralBattle();
		}

		private void button9_Click(object sender, EventArgs e)
		{
			Core.StartROI();
		}

		private void button10_Click(object sender, EventArgs e)
		{
			Core.StartDrawOtcIndicators();
		}
	}
}

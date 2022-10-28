﻿namespace AbsurdMoneySimulations
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

		private void button4_Click(object sender, EventArgs e)
		{
			API.TraderTrade();
		}

		private void MainForm_Load_1(object sender, EventArgs e)
		{
			Core.OnAppStarting();
		}

		private void button6_Click(object sender, EventArgs e)
		{
			API.RecreateNN();
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
			API.FitByBackPropgation();
		}

		private void button8_Click(object sender, EventArgs e)
		{
			API.NeuralBattle();
		}

		private void button9_Click(object sender, EventArgs e)
		{
			API.FindSwarmStatistics();
		}

		private void button10_Click(object sender, EventArgs e)
		{
			API.DrawOtcIndicators();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			API.FindDetailedSectionsStatistics();
		}

		private void button11_Click(object sender, EventArgs e)
		{
			API.FitSwarm();
		}

		private void button12_Click(object sender, EventArgs e)
		{
			API.RecreateSwarm();
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			
		}

		private void button13_Click(object sender, EventArgs e)
		{
			API.LiveGraphGetting();
		}

		private void button14_Click(object sender, EventArgs e)
		{
			API.OpenQtxOnly();
		}

		private void button15_Click(object sender, EventArgs e)
		{
			API.DeleteCookies();
		}
	}
}

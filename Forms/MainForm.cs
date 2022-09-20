﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
			FormsManager.OpenMartingaleForm();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			MinimalWinrateFinder.Do();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Tests.StartTest();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{

		}
	}
}

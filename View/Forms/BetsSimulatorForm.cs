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

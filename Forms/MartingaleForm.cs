using System;
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
	public partial class MartingaleForm : Form
	{
		public MartingaleForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			MartingaleSimulator.Simulate();
		}
	}
}

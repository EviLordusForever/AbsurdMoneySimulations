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
	public partial class FittingParamsForm : Form
	{
		public FittingParams _fp;

		public FittingParamsForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			_fp = GetFittingParams();
		}

		private FittingParams GetFittingParams()
		{
			FittingParams fp = new FittingParams();

			fp._LEARINING_RATE = Convert.ToSingle(LEARNING_RATE.Text);
			fp._MOMENTUM = Convert.ToSingle(MOMENTUM.Text);
			fp._trainingTestsCount = Convert.ToInt32(trainingTestsCount.Text);
			fp._validationTestsCount = Convert.ToInt32(validationTestsCount.Text);
			fp._batchSize = Convert.ToInt32(batchSize.Text);
			fp._validationRecalculatePeriod = Convert.ToInt32(validationRecalculatePeriod.Text);
			fp._statisticsRecalculatePeriod = Convert.ToInt32(statisticsRecalculatePeriod.Text);
			fp._useDropout = Convert.ToBoolean(useDropout.Text);

			return fp;
		}

		private void FittingParamsForm_FormClosing(object sender, FormClosingEventArgs e)
		{
		}

		private void FittingParamsForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_fp == null)
			{
				FormsManager._fittingParamsForm = null;
				FormsManager.OpenFittingParamsForm(GetFittingParams());
			}
		}

		private void FittingParamsForm_Load(object sender, EventArgs e)
		{

		}
	}
}

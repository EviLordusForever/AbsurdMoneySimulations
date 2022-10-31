namespace AbsurdMoneySimulations
{
	public static class FormsManager
	{
		public static MainForm _mainForm;
		public static BetsSimulatorForm _betsSimulatorForm;
		public static ShowForm _showForm;
		public static LogForm _logForm;
		public static PredictionForm _predictionForm;
		public static TraderReportForm _traderReportForm;
		public static FittingParamsForm _fittingParamsForm;

		public static void ShowImage(Image img)
		{
			ShowImage((Bitmap)img);
		}

		public static void ShowImage(Bitmap bmp)
		{
			if (_showForm == null || _showForm.IsDisposed)
				OpenShowForm("aboba");

			Bitmap bmp0 = Library.Graphics2.RescaleBitmap(bmp, _showForm.ClientSize.Width, _showForm.ClientSize.Height);

			_mainForm.Invoke(new Action(() =>
			{
				_showForm.BackgroundImage = bmp0;
			}));
		}

		public static void ShowImageToPredictionForm(Bitmap bmp)
		{
			if (_showForm == null || _showForm.IsDisposed)
				OpenShowForm("aboba");

			Bitmap bmp0 = Library.Graphics2.RescaleBitmap(bmp, _predictionForm.ClientSize.Width, _predictionForm.ClientSize.Height);

			_mainForm.Invoke(new Action(() =>
			{
				_predictionForm.BackgroundImage = bmp0;
			}));
		}

		public static void OpenBetsSimulatorForm()
		{
			_mainForm.Invoke(new Action(() =>
			{
				if (_betsSimulatorForm == null || _betsSimulatorForm.IsDisposed)
					_betsSimulatorForm = new BetsSimulatorForm();

				_betsSimulatorForm.Show();
				_betsSimulatorForm.WindowState = FormWindowState.Normal;
				_betsSimulatorForm.BringToFront();
			}));
		}

		public static void OpenPredictionForm()
		{
			_mainForm.Invoke(new Action(() =>
			{
				if (_predictionForm == null || _predictionForm.IsDisposed)
					_predictionForm = new PredictionForm();

				_predictionForm.Show();
				_predictionForm.WindowState = FormWindowState.Normal;
				Rectangle bounds = Screen.PrimaryScreen.Bounds;
				_predictionForm.Location = new Point((bounds.Width - _predictionForm.Width) / 2, 0);
				_predictionForm.BringToFront();
				_predictionForm.TopMost = true;
			}));
		}

		public static void OpenTraderReportForm()
		{
			_mainForm.Invoke(new Action(() =>
			{
				if (_traderReportForm == null || _traderReportForm.IsDisposed)
					_traderReportForm = new TraderReportForm();

				_traderReportForm.Show();
				_traderReportForm.WindowState = FormWindowState.Normal;
				_traderReportForm.BringToFront();
				_traderReportForm.TopMost = true;
				_traderReportForm.Location = new Point(0, 0);
			}));
		}

		public static void OpenShowForm(string text)
		{
			_mainForm.Invoke(new Action(() =>
			{
				if (_showForm == null || _showForm.IsDisposed)
				{
					_showForm = new ShowForm();
				}

				_showForm.Show();
				_showForm.WindowState = FormWindowState.Normal;
				_showForm.BringToFront();
				_showForm.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
				_showForm.Location = new Point(0, 0);
				_showForm.BackgroundImageLayout = ImageLayout.Stretch;

				_showForm.Text = text;
			}));
		}

		public static void OpenLogForm()
		{
			_mainForm.Invoke(new Action(() =>
			{
				if (_logForm == null || _logForm.IsDisposed)
				{
					_logForm = new LogForm();
					_logForm.BringToFront();
					_logForm.Show();
					_logForm.WindowState = FormWindowState.Normal;
					_logForm.Location = new Point(-7, 0);
					_logForm.rtb.ForeColor = Color.FromArgb(0, 255, 0);
				}
			}));
		}

		public static FittingParams AskFittingParams(FittingParams fp)
		{
			OpenFittingParamsForm(fp);
			while (_fittingParamsForm == null || _fittingParamsForm._fp == null)
				Thread.Sleep(100);
			fp = _fittingParamsForm._fp;
			CloseForm(_fittingParamsForm);
			return fp;
		}

		public static void OpenFittingParamsForm(FittingParams fp)
		{
			_mainForm.Invoke(new Action(() =>
			{
				if (_fittingParamsForm == null || _fittingParamsForm.IsDisposed)
				{
					_fittingParamsForm = new FittingParamsForm();
					_fittingParamsForm.BringToFront();
					_fittingParamsForm.Show();
					_fittingParamsForm.WindowState = FormWindowState.Normal;
				}

				_fittingParamsForm.LEARNING_RATE.Text = fp._LEARINING_RATE.ToString();
				_fittingParamsForm.MOMENTUM.Text = fp._MOMENTUM.ToString().ToString();
				_fittingParamsForm.trainingTestsCount.Text = fp._trainingTestsCount.ToString();
				_fittingParamsForm.validationRecalculatePeriod.Text = fp._validationTestsCount.ToString();
				_fittingParamsForm.batchSize.Text = fp._batchSize.ToString();
				_fittingParamsForm.validationRecalculatePeriod.Text = fp._validationRecalculatePeriod.ToString();
				_fittingParamsForm.statisticsRecalculatePeriod.Text = fp._statisticsRecalculatePeriod.ToString();
				_fittingParamsForm.useDropout.Text = fp._useDropout.ToString();
			}));
		}

		public static void SetShowFormSize(int w, int h)
		{
			_mainForm.Invoke(new Action(() =>
			{
				_showForm.Size = new Size(w, h);
			}));
		}

		public static void SayToTraderReport1(string text)
		{
			if (_traderReportForm == null || _traderReportForm.IsDisposed)
				OpenTraderReportForm();

			_mainForm.Invoke(new Action(() =>
			{
				_traderReportForm.rtb1.Text = text;
			}));
		}

		public static void SayToTraderReport2(string text)
		{
			if (_traderReportForm == null || _traderReportForm.IsDisposed)
				OpenTraderReportForm();

			_mainForm.Invoke(new Action(() =>
			{
				_traderReportForm.rtb2.Text = text;
			}));
		}

		public static void HideForm(Form form)
		{
			_mainForm.Invoke(new Action(() =>
			{
				form.Hide();
			}));
		}

		public static void CloseForm(Form form)
		{
			_mainForm.Invoke(new Action(() =>
			{
				form.Close();
			}));
		}

		public static void BringToFrontForm(Form form)
		{
			_mainForm.Invoke(new Action(() =>
			{
				form.BringToFront();
			}));
		}

		public static void UnhideForm(Form form)
		{
			_mainForm.Invoke(new Action(() =>
			{
				form.Show();
			}));
		}

		public static void MoveFormToCenter(Form form)
		{
			_mainForm.Invoke(new Action(() =>
			{
				Rectangle bounds = Screen.PrimaryScreen.Bounds;
				form.Location = new Point((bounds.Width - form.Width)/2, (bounds.Height - form.Height) / 2);
			}));
		}

		public static void MoveFormToCenterX(Form form)
		{
			_mainForm.Invoke(new Action(() =>
			{
				Rectangle bounds = Screen.PrimaryScreen.Bounds;
				form.Location = new Point((bounds.Width - form.Width) / 2, 0);
			}));
		}
	}
}

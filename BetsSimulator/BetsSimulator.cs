using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AbsurdMoneySimulations
{
	public static class BetsSimulator
	{
		public static void Simulate()
		{
			FormsManager.OpenShowForm("Bets simulation");

			Thread myThread = new Thread(SimulateThread);
			myThread.Name = "BetsSimulator";
			myThread.Start();

			void SimulateThread()
			{
				int betsCount = Convert.ToInt16(FormsManager.betsSimulatorForm.betsCount.Text);
				int heigh = Convert.ToInt16(FormsManager.betsSimulatorForm.height.Text);
				Storage.bmp = new Bitmap(betsCount, heigh);

				Graphics gr = Graphics.FromImage(Storage.bmp);
				gr.Clear(Color.White);

				int martingaleChain = Convert.ToInt16(FormsManager.betsSimulatorForm.martingaleChain.Text);
				int antimaringaleChain = Convert.ToInt16(FormsManager.betsSimulatorForm.antimartingaleChain.Text);
				double startMoney = Convert.ToDouble(FormsManager.betsSimulatorForm.money.Text);
				double betPercent = Convert.ToDouble(FormsManager.betsSimulatorForm.betPercent.Text) / 100;
				double prize = Convert.ToDouble(FormsManager.betsSimulatorForm.prize.Text) / 100;
				double winrate = Convert.ToDouble(FormsManager.betsSimulatorForm.winrate.Text) / 100;
				double simulationsCount = Convert.ToDouble(FormsManager.betsSimulatorForm.simulationsCount.Text);
				double money;
				double oldmoney;
				double[] avarageMoney = new double[betsCount];
				double[] risk = new double[betsCount];
				Pen pen;
				bool win = true;
				bool winBefore = true;
				int y;
				int y0;
				double lastProfit = 0;
				int winCombo = 0;
				int looseCombo = 0;
				double lastBet = 0;
				double loosedProfit = 0;

				for (int s = 0; s < simulationsCount; s++)
				{
					money = startMoney;
					oldmoney = money;

					pen = new Pen(Color.FromArgb(Storage.rnd.Next(255), Storage.rnd.Next(255), Storage.rnd.Next(255)));

					for (int b = 0; b < betsCount; b++)
					{
						y0 = (int)(heigh - 300 - money);

						Play();


						avarageMoney[b] += money;

						if (money > startMoney)
							risk[b]++;
						else
							risk[b]--;

						y = (int)(heigh - 300 - money);

						if (y > -5000000 && y0 > -5000000)
							if (y < 5000000 && y0 < 5000000)
								gr.DrawLine(pen, b - 1, y0, b, y);
					}
				}

				int y1 = (int)(heigh - 300 - startMoney);
				pen = new Pen(Color.Green, 2);
				gr.DrawLine(Pens.Black, 0, y1, betsCount, y1);
				y1 = heigh - 300;
				gr.DrawLine(Pens.Black, 0, y1, betsCount, y1);

				Brush br = new SolidBrush(Color.FromArgb(50, 0, 255, 0));
				gr.FillRectangle(br, 0, heigh - 130, betsCount, 100);
				gr.DrawLine(pen, 0, heigh - 130, betsCount, heigh - 130);
				gr.DrawLine(pen, 0, heigh - 30, betsCount, heigh - 30);
				gr.DrawLine(Pens.Green, 0, heigh - 80, betsCount, heigh - 80);


				pen = new Pen(Color.Black, 5);
				for (int i = 1; i < betsCount; i++)
				{
					y0 = (int)(heigh - 300 - avarageMoney[i - 1] / simulationsCount);
					y = (int)(heigh - 300 - avarageMoney[i] / simulationsCount);

					if (y > -1000000 && y0 > -1000000)
						if (y < 1000000 && y0 < 1000000)
							gr.DrawLine(pen, i - 1, y0, i, y);

					y0 = (int)(heigh - 80 - 50 * risk[i - 1] / simulationsCount);
					y = (int)(heigh - 80 - 50 * risk[i] / simulationsCount);

					gr.DrawLine(Pens.Red, i - 1, y0, i, y);
				}

				Storage.bmp = Extensions.RescaleBitmap(Storage.bmp, FormsManager.showForm.ClientSize.Width, Storage.bmp.Height);
				gr = Graphics.FromImage(Storage.bmp);
				gr.DrawString("Not loosers, %:", new Font("Tahoma", 14), Brushes.Black, 5, heigh - 156);
				gr.DrawString($"So, {Math.Round(100.0 * (risk[betsCount - 1] + simulationsCount) / (2.0 * simulationsCount), 2)}% of simulations are in profit.", new Font("Tahoma", 14), Brushes.Black, Storage.bmp.Width - 340, heigh - 156);
				gr.FillRectangle(Brushes.Cyan, Storage.bmp.Width - 270, 17, 270, 26);
				gr.FillRectangle(Brushes.Red, Storage.bmp.Width - 270, 17 + 27, 270, 26);
				double ap = Math.Round(avarageMoney[betsCount - 1] / simulationsCount - startMoney, 2);
				string apStr = ap.ToString() + "$";
				if (ap > 10000000)
					apStr = "fucking ∞";
				if (ap < -10000000)
					apStr = "fucking -∞";

				gr.DrawString($"Average profit: {apStr}", new Font("Tahoma", 14), Brushes.Black, Storage.bmp.Width - 270, 17);
				gr.DrawString($"After {betsCount} bets", new Font("Tahoma", 14), Brushes.White, Storage.bmp.Width - 270, 17 + 27);

				Storage.bmp = Extensions.RescaleBitmap(Storage.bmp, Storage.bmp.Width, FormsManager.showForm.ClientSize.Height);

				FormsManager.mainForm.Invoke(new Action(() =>
				{
					FormsManager.showForm.BackgroundImage = Storage.bmp;
					FormsManager.betsSimulatorForm.BringToFront();
				}));

				void Play()
				{
					lastBet = CalculateBet();

					winBefore = win;
					win = Storage.rnd.NextDouble() < winrate;

					if (win)
					{
						lastProfit = lastBet * prize;
						loosedProfit = 0;
					}
					else
					{
						lastProfit = -lastBet * 1.00;

						if (winBefore)
							loosedProfit += lastBet * prize;

						loosedProfit += lastBet * 1.00;
					}

					money += lastProfit;
				}

				double CalculateBet()
				{
					if (win)
					{
						winCombo++;
						looseCombo = 0;
					}
					else
					{
						looseCombo++;
						winCombo = 0;
					}

					double res = 0;

					if (winCombo > 0 && winCombo <= antimaringaleChain)
						res = lastBet + lastProfit;
					else
					if (looseCombo > 0 && looseCombo <= martingaleChain)
						res = loosedProfit / prize * 1;
					else
						res = Math.Max(1, money * betPercent);

					return res;
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AbsurdMoneySimulations
{
	public static class MartingaleSimulator
	{
		public static void Simulate()
		{
			FormsManager.OpenShowForm();

			Thread myThread = new Thread(SimulateThread);
			myThread.Name = "Martingale";
			myThread.Start();

			void SimulateThread()
			{	
				Graphics gr = Graphics.FromImage(FormsManager.showForm.bmp);

				double money = Convert.ToDouble(FormsManager.martingaleForm.money.Text);
				double bet = Convert.ToDouble(FormsManager.martingaleForm.bet.Text) / 100;
				double prize = Convert.ToDouble(FormsManager.martingaleForm.prize.Text) / 100;
				double winrate = Convert.ToDouble(FormsManager.martingaleForm.winrate.Text) / 100;

				for (int i = 0; i < 1920; i++)
				{
					Play();

					int y = (int)(1080 - 300 - money);

					if (y < FormsManager.showForm.bmp.Height && y >= 0)
						FormsManager.showForm.bmp.SetPixel(i, y, Color.Black);
				}

				FormsManager.mainForm.Invoke(new Action(() =>
				{
					FormsManager.showForm.Refresh();
					FormsManager.martingaleForm.BringToFront();
				}));				

				void Play()
				{
					double actualBet = Math.Max(1, money * bet);
					money -= actualBet;

					if (Storage.rnd.NextDouble() < winrate)
						money += actualBet * (1 + prize);
				}
			}
		}
	}
}

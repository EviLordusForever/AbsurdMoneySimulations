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
			Thread myThread = new Thread(SimulateThread);
			myThread.Name = "Martingale";
			myThread.Start();

			void SimulateThread()
			{
				Bitmap bmp = new Bitmap(1920, 1080);
				Graphics gr = Graphics.FromImage(bmp);

				double money = Convert.ToDouble(FormsManager.martingaleForm.money.Text);
				double bet = Convert.ToDouble(FormsManager.martingaleForm.bet.Text) / 100;
				double prize = Convert.ToDouble(FormsManager.martingaleForm.prize.Text) / 100;
				double winrate = Convert.ToDouble(FormsManager.martingaleForm.winrate.Text) / 100;

				for (int i = 0; i < 999999; i++)
				{
					Play();

					MessageBox.Show($"{money}");
					bmp.SetPixel(i, (int)(1280 - 300 - money), Color.Black);
				}

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

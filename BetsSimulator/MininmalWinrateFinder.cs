using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class MinimalWinrateFinder
	{
		public static void MakeTable()
		{
			FormsManager.OpenShowForm();
			FormsManager.showForm.Text = "winrate, prize and profit table";

			double[,] table = new double[120, 120];

			Storage.bmp = new Bitmap(2700, 1920);
			Graphics gr = Graphics.FromImage(Storage.bmp);


			for (double wr = 45; wr <= 110; wr += 1.0 / 26)
			{
				double prize = 100 * (100 - wr) / wr;
				gr.DrawLine(Pens.LimeGreen, (int)(40 + wr * 26), (int)(50 + prize * 18), (int)(40 + wr * 26), 1920);
				gr.DrawLine(Pens.Red, (int)(40 + wr * 26), (int)(50 + prize * 18), (int)(40 + wr * 26), 30);
			}
			gr.FillRectangle(Brushes.Red, 50, 33, 40 + 45 * 26, 1920);

			for (int wr = 0; wr <= 100; wr += 2)
			{
				for (int prize = 0; prize <= 100; prize += 2)
				{
					table[wr, prize] = wr / 100.0 * (prize / 100.0) - (1 - wr / 100.0);
					gr.DrawString($"{Math.Round(table[wr, prize], 2)}", new Font("Tahoma", 14), Brushes.Black, 50 + wr * 26, 40 + prize * 18);
				}
			}

			for (int prize = 0; prize <= 100; prize += 2)
				gr.DrawString($"{Math.Round(prize / 100.0, 2)}", new Font("Tahoma", 14), Brushes.Blue, 53 + -2 * 26, 40 + prize * 18);

			for (int wr = 0; wr <= 100; wr += 2)
				gr.DrawString($"{Math.Round(wr / 100.0, 2)}", new Font("Tahoma", 14), Brushes.Magenta, 53 + wr * 26, 40 + -2 * 18);


			for (int wr = 0; wr <= 100; wr += 2)
				gr.DrawLine(Pens.Black, 50 + wr * 26 - 2, 0, 50 + wr * 26, 1920);

			for (int prize = 0; prize <= 100; prize += 2)
				gr.DrawLine(Pens.Black, 0, 33 + prize * 18, 2700, 33 + prize * 18);


			FormsManager.mainForm.Invoke(new Action(() =>
			{
				FormsManager.showForm.BackgroundImage = BetsSimulator.GetFormBackgroundImage(Storage.bmp, FormsManager.showForm.ClientSize.Width, FormsManager.showForm.ClientSize.Height);
			}));
		}
	}
}

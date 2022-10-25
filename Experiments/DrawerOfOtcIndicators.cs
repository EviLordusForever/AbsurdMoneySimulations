using System;
using Library;

namespace AbsurdMoneySimulations
{
	public class DrawerOfOtcIndicators
	{
		public void Draw(float[] grafic)
		{
			ActivationFunction af = new SoftSign();
			Bitmap bmp = new Bitmap(grafic.Length, 1100);
			Graphics gr = Graphics.FromImage(bmp);
			int xScale = 3;

			gr.Clear(Color.White);
			DrawIndicator1(Pens.Green, -30, 50);
			DrawIndicator1(Pens.Red, 30, 150);
			DrawIndicator1(Pens.Red, 60, 250);
			DrawIndicator1(Pens.Red, 90, 350);
			DrawIndicator1(Pens.Red, 120, 450);
			DrawIndicator1(Pens.Red, 150, 550);
			DrawIndicator1(Pens.Red, 180, 650);
			DrawIndicator1(Pens.Red, 210, 750);
			DrawIndicator1(Pens.Red, 240, 850);
			DrawIndicator1(Pens.Red, 270, 950);
			DrawIndicator1(Pens.Red, 300, 1050);

			for (int i = 0; i < grafic.Length; i += 30)
				gr.DrawLine(Pens.Black, i, 0, i, bmp.Height);

			Disk2.SaveImageToProgramFiles(bmp, "OTC.bmp");
			Logger.Log("done #1");

			//////////////////////////////

			bmp = new Bitmap(grafic.Length * xScale, 100);
			gr = Graphics.FromImage(bmp);
			gr.Clear(Color.White);

			Pen pen = new Pen(Color.FromArgb(255, 255, 0, 0), 1);
			DrawIndicator2(pen, 30);
			pen = new Pen(Color.FromArgb(235, 255, 0, 0), 1);
			DrawIndicator2(pen, 60);
			pen = new Pen(Color.FromArgb(215, 255, 0, 0), 1);
			DrawIndicator2(pen, 90);
			pen = new Pen(Color.FromArgb(195, 255, 0, 0), 1);
			DrawIndicator2(pen, 120);
			pen = new Pen(Color.FromArgb(175, 255, 0, 0), 1);
			DrawIndicator2(pen, 150);
			pen = new Pen(Color.FromArgb(155, 255, 0, 0), 1);
			DrawIndicator2(pen, 180);
			pen = new Pen(Color.FromArgb(135, 255, 0, 0), 1);
			DrawIndicator2(pen, 210);
			pen = new Pen(Color.FromArgb(115, 255, 0, 0), 1);
			DrawIndicator2(pen, 240);
			pen = new Pen(Color.FromArgb(95, 255, 0, 0), 1);
			DrawIndicator2(pen, 270);
			pen = new Pen(Color.FromArgb(85, 255, 0, 0), 1);
			DrawIndicator2(pen, 300);

			gr.DrawLine(Pens.Black, 0, 50, bmp.Width, 50);

			DrawIndicator2(Pens.Green, -30);

			DrawAverageOfIndicators2();

			Disk2.SaveImageToProgramFiles(bmp, "OTC2.bmp");
			Logger.Log("done #2");


			void DrawIndicator1(Pen pen, int gOffset, int yOffset)
			{
				for (int v = 0; v < grafic.Length; v++)
				{
					float point = af.f(Differense(v, gOffset) / 15f) * 50;
					gr.DrawLine(pen, v, yOffset, v, yOffset - point);
				}
			}

			void DrawIndicator2(Pen pen, int gOffset)
			{
				float oldY = 0;
				for (int v = 0; v < grafic.Length; v++)
				{
					float y = af.f(Differense(v, gOffset) / 15f) * 50;

					gr.DrawLine(pen, (v-1)*xScale, 50 - oldY, v*xScale, 50 - y);
					oldY = y;
				}
			}

			void DrawAverageOfIndicators2()
			{
				float oldY = 0;
				for (int v = 0; v < grafic.Length; v++)
				{
					float y = Differense(v, 30) * 512;
					y += Differense(v, 60) * 256;
					y += Differense(v, 90) * 128;
					y += Differense(v, 120) * 64;
					y += Differense(v, 180) * 32;
					y += Differense(v, 210) * 16;
					y += Differense(v, 240) * 8;
					y += Differense(v, 270) * 4;
					y += Differense(v, 300) * 2;
					y /= 512 + 256 + 128 + 64 + 32 + 16 + 8 + 4 + 2;
					y = af.f(y / 15f) * 50;

					gr.DrawLine(Pens.Blue, (v - 1) * xScale, 50 - oldY, v * xScale, 50 - y);
					oldY = y;
				}
			}


			float Differense(int point, int offset)
			{
				if (point - offset >= 0 && point - offset < grafic.Length)
					return grafic[point] - grafic[point - offset];
				else
					return 0;
			}
		}
	}
}

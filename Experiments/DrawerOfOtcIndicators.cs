using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class DrawerOfOtcIndicators
	{
		public void Draw(float[] grafic)
		{
			ActivationFunction af = new SoftSign();
			Bitmap bmp = new Bitmap(grafic.Length, 1100);
			Graphics gr = Graphics.FromImage(bmp);
			int d = 3;


			gr.Clear(Color.White);
			DrawOne(Pens.Green, -30, 50);
			DrawOne(Pens.Red, 30, 150);
			DrawOne(Pens.Red, 60, 250);
			DrawOne(Pens.Red, 90, 350);
			DrawOne(Pens.Red, 120, 450);
			DrawOne(Pens.Red, 150, 550);
			DrawOne(Pens.Red, 180, 650);
			DrawOne(Pens.Red, 210, 750);
			DrawOne(Pens.Red, 240, 850);
			DrawOne(Pens.Red, 270, 950);
			DrawOne(Pens.Red, 300, 1050);

			for (int i = 0; i < grafic.Length; i += 30)
				gr.DrawLine(Pens.Black, i, 0, i, bmp.Height);

			Disk.SaveImageToProgramFiles(bmp, "OTC.bmp");
			Logger.Log("done #1");

			//////////////////////////////

			bmp = new Bitmap(grafic.Length * d, 100);
			gr = Graphics.FromImage(bmp);

			gr.Clear(Color.White);

			Pen pen = new Pen(Color.FromArgb(255, 255, 0, 0), 1);
			DrawSecond(pen, 30);
			pen = new Pen(Color.FromArgb(235, 255, 0, 0), 1);
			DrawSecond(pen, 60);
			pen = new Pen(Color.FromArgb(215, 255, 0, 0), 1);
			DrawSecond(pen, 90);
			pen = new Pen(Color.FromArgb(195, 255, 0, 0), 1);
			DrawSecond(pen, 120);
			pen = new Pen(Color.FromArgb(175, 255, 0, 0), 1);
			DrawSecond(pen, 150);
			pen = new Pen(Color.FromArgb(155, 255, 0, 0), 1);
			DrawSecond(pen, 180);
			pen = new Pen(Color.FromArgb(135, 255, 0, 0), 1);
			DrawSecond(pen, 210);
			pen = new Pen(Color.FromArgb(115, 255, 0, 0), 1);
			DrawSecond(pen, 240);
			pen = new Pen(Color.FromArgb(95, 255, 0, 0), 1);
			DrawSecond(pen, 270);
			pen = new Pen(Color.FromArgb(85, 255, 0, 0), 1);
			DrawSecond(pen, 300);

			gr.DrawLine(Pens.Black, 0, 50, bmp.Width, 50);

			DrawSecond(Pens.Green, -30);

			DrawThird();


			Disk.SaveImageToProgramFiles(bmp, "OTC2.bmp");
			Logger.Log("done #2");

			void DrawOne(Pen pen, int gOffset, int yOffset)
			{
				for (int v = 0; v < grafic.Length; v++)
				{
					float point = af.f(Differense(v, gOffset) / 15f) * 50;
					gr.DrawLine(pen, v, yOffset, v, yOffset - point);
				}
			}

			void DrawSecond(Pen pen, int gOffset)
			{
				float oldY = 0;
				for (int v = 0; v < grafic.Length; v++)
				{
					float y = af.f(Differense(v, gOffset) / 15f) * 50;

					gr.DrawLine(pen, (v-1)*d, 50 - oldY, v*d, 50 - y);
					oldY = y;
				}
			}

			void DrawThird()
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

					gr.DrawLine(Pens.Blue, (v - 1) * d, 50 - oldY, v * d, 50 - y);
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

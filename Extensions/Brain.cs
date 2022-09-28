using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class Brain
	{
		public static double Interpolate(double from, double to, double actual)
		{
			return (actual - from) / (to - from);
		}

		public static double Normalize(double input)
		{
			return (double)(2 * 1 / (1 + Math.Pow(1.1, -input)) - 1);
		}

		public static float Normalize(float input)
		{
			float a = (2f * 1 / (1 + MathF.Pow(1.1f, -input)) - 1);
			return a;
		}

		public static T[] SubArray<T>(this T[] array, int offset, int length)
		{
			return array.Skip(offset)
						.Take(length)
						.ToArray();
		}

		public static Bitmap RescaleBitmap (Bitmap bmp0, int width, int height)
		{
			Bitmap bmp = new Bitmap(width, height);
			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.DrawImage(bmp0, new Rectangle(0, 0, bmp.Width, bmp.Height));
			}
			return bmp;
		}
	}
}

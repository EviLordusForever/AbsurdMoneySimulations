using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class Extensions
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
			float a = (2f / (1 + MathF.Pow(1.1f, -input)) - 1);
			return a;
		}

		public static T[] SubArray<T>(this T[] array, int offset, int length)
		{
			return array.Skip(offset)
						.Take(length)
						.ToArray();
		}

		public static Bitmap RescaleBitmap(Bitmap bmp0, int width, int height)
		{
			Bitmap bmp = new Bitmap(width, height);
			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.DrawImage(bmp0, new Rectangle(0, 0, bmp.Width, bmp.Height));
			}
			return bmp;
		}

		public static float Max(float[] array)
		{
			float max = array[0];
			for (int i = 1; i < array.Length; i++)
				if (array[i] > max)
					max = array[i];
			return max;
		}

		public static float Min(float[] array)
		{
			float min = array[0];
			for (int i = 1; i < array.Length; i++)
				if (array[i] < min)
					min = array[i];
			return min;
		}

		public static float NormalD(float scale, float centralization, float lowing)
		{
			//centalization = 3, 5, 7, 19, ect.
			float x = Storage.rnd.NextSingle();
			int sign = (Storage.rnd.Next(2) * 2 - 1);
			float a = (MathF.Pow(x, centralization) + x * lowing) / (lowing + 1);

			return scale * a * sign;
		}

		public static T[] Convert2DArrayTo1D<T>(T[][] array2D)
		{
			List<T> lst = new List<T>();
			foreach (T[] a in array2D)
			{
				lst.AddRange(a);
			}
			return lst.ToArray();
		}
	}
}

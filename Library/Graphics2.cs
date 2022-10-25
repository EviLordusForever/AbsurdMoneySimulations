using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
	public static class Graphics2
	{
		public static Bitmap RescaleBitmap(Bitmap bmp0, int width, int height)
		{
			Bitmap bmp = new Bitmap(width, height);
			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.DrawImage(bmp0, new Rectangle(0, 0, bmp.Width, bmp.Height));
			}
			return bmp;
		}

		public static RotateFlipType RotateFlipTypeRandom
		{
			get
			{
				int n = Math2.rnd.Next(16);
				switch (n)
				{
					case 0:
						return RotateFlipType.Rotate180FlipNone;
					case 1:
						return RotateFlipType.Rotate180FlipX;
					case 2:
						return RotateFlipType.Rotate180FlipXY;
					case 3:
						return RotateFlipType.Rotate180FlipY;

					case 4:
						return RotateFlipType.Rotate270FlipNone;
					case 5:
						return RotateFlipType.Rotate270FlipX;
					case 6:
						return RotateFlipType.Rotate270FlipXY;
					case 7:
						return RotateFlipType.Rotate270FlipY;

					case 8:
						return RotateFlipType.Rotate90FlipNone;
					case 9:
						return RotateFlipType.Rotate90FlipX;
					case 10:
						return RotateFlipType.Rotate90FlipXY;
					case 11:
						return RotateFlipType.Rotate90FlipY;

					case 12:
						return RotateFlipType.RotateNoneFlipNone;
					case 13:
						return RotateFlipType.RotateNoneFlipX;
					case 14:
						return RotateFlipType.RotateNoneFlipXY;
					case 15:
						return RotateFlipType.RotateNoneFlipY;

					default:
						throw new Exception();
				}
			}
		}
	}
}

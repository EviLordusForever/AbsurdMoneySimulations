using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class FakeOTC2
	{
		public static List<float> _history;
		public static float _money = 1;
		public static int _heigh = 1080;
		public static int _width = 1920;
		public static Bitmap _bmp = new Bitmap(_width, _heigh);
		public static Graphics _gr = Graphics.FromImage(_bmp);
		public static Bitmap _bmp2 = new Bitmap(_width, _heigh);
		public static Graphics _gr2 = Graphics.FromImage(_bmp2);
		public static int yscale = 10;
		public static float offset = 0;
		public static int offsetDelay = 0;
		public static float speed = 0;
		public static float speedDelay = 0;

		public static void DO()
		{
			Thread myThread = new Thread(MyThread);
			myThread.Name = "Fake OTC";
			myThread.Start();

			void MyThread()
			{
				_history = new List<float>();

				for (int i = 0; i < 60; i++)
					_history.Add(1);

				_gr.Clear(Color.Black);

				for (int time = 0; ; time++)
				{
					CorrectMoney(time);
					Draw(time);
					
					_history.Add(_money);
				}
			}
		}

		public static void Draw(int time)
		{
			if (_history.Count < 5)
				return;
			
			int x1 = _width / 2 - 2 + time % 15;
			int x2 = _width / 2 - 1 + time % 15;
			int y1 = (int)(_heigh / 2 - _history[_history.Count - 2] * yscale);
			int y2 = (int)(_heigh / 2 - _history[_history.Count - 1] * yscale);
			_gr.DrawLine(Pens.Cyan, x1, y1, x2, y2);

			if ((time + 1) % 15 == 0)
			{
				_gr.DrawImage(_bmp, new Rectangle(-15, 0, _bmp.Width, _bmp.Height), new Rectangle(0, 0, _bmp.Width, _bmp.Height), GraphicsUnit.Pixel);
				_gr2.DrawImage(_bmp, 0, 0, _bmp.Width, _bmp.Height);
				FormsManager.ShowImage(_bmp2);
			}
		}

		public static void CorrectMoney(int time)
		{
			offsetDelay--;
			if (offsetDelay <= 0)
			{
				offset = (Math2.rnd.NextSingle() - 0.5f) * 10;
				offsetDelay = 10 + Math2.rnd.Next(50);
				//Logger.Log(offset);
			}

			speedDelay--;
			if (speedDelay <= 0)
			{
				speed = (Math2.rnd.NextSingle() - 0.5f) * 1;
				speedDelay = Math2.rnd.Next(15);
				//Logger.Log(offset);
			}
			float aim = _history[_history.Count - 60] + offset;

			if (aim > 15)
				aim = 15;
			if (aim < -15)
				aim = -15;

			//Logger.Log(aim);

			if (aim > _money)
				_money = _money + speed;
			else
				_money = _money - speed;
		}		
	}
}


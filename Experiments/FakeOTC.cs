using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class FakeOTC
	{
		public static List<float> _history;
		public static float _money = 1;
		public static List<Bet> _bets;
		public static int _heigh = 1080;
		public static int _width = 1920;
		public static Bitmap _bmp = new Bitmap(_width, _heigh);
		public static Graphics _gr = Graphics.FromImage(_bmp);
		public static int yscale = 10;

		public static void DO()
		{
			Thread myThread = new Thread(MyThread);
			myThread.Name = "Fake OTC";
			myThread.Start();

			void MyThread()
			{
				_bets = new List<Bet>();
				_history = new List<float>();

				_gr.Clear(Color.Black);

				for (int time = 0; ; time++)
				{
					FinishBets(time);
					AddBets(time);
					CorrectMoney(time);
					Draw(time);
					FormsManager.ShowImage(_bmp);
					//Thread.Sleep(100);
					_history.Add(_money);
				}
			}
		}

		public static void Draw(int time)
		{
			if (_history.Count < 5)
				return;

			_gr.DrawImage(_bmp, new Rectangle(-1, 0, _bmp.Width, _bmp.Height), new Rectangle(0, 0, _bmp.Width, _bmp.Height), GraphicsUnit.Pixel);

			int x1 = _width / 2 - 2;
			int x2 = _width / 2 - 1;
			int y1 = (int)(_heigh / 2 - _history[_history.Count - 2] * yscale);
			int y2 = (int)(_heigh / 2 - _history[_history.Count - 1] * yscale);
			_gr.DrawLine(Pens.Cyan, x1, y1, x2, y2);
		}

		public static void CorrectMoney(int time)
		{
			if (_bets.Count == 0)
				return;

			int minTime = _bets[0]._endTime;
			int idOfMin = 0;

			for (int id = 0; id < _bets.Count; id++)
				if (_bets[id]._endTime < minTime)
					idOfMin = id;

			_history.Add(_money);

			float aim = _bets[idOfMin]._price + 5;
			if (!_bets[idOfMin]._up)
				aim = _bets[idOfMin]._price - 5;

			_money += 2 * Math2.rnd.NextSingle() * (aim - _money) / (_bets[idOfMin]._endTime - time);
		}

		public static void AddBets(int time)
		{
			if (Math2.rnd.Next(60) == 0)
			{
				bool up = true;
				if (Math2.rnd.Next(2) == 0)
					up = false;

				_bets.Add(new Bet(time, time + 60, _money, up));
				Logger.Log($"bet {time} - {time + 60}");

				int x1 = _width / 2;
				int x2 = _width / 2 + 60;
				int y = _heigh / 2 - (int)_bets[_bets.Count - 1]._price * yscale;

				if (_bets[_bets.Count - 1]._up)
					_gr.DrawLine(Pens.Green, x1, y, x2, y);
				else
					_gr.DrawLine(Pens.Red, x1, y, x2, y);
			}
		}

		public static void FinishBets(int time)
		{
			for (int id = 0; id < _bets.Count; id++)
				if (_bets[id]._endTime <= time)
					_bets.RemoveAt(id);
		}
	}

	public class Bet
	{
		public int _startTime;
		public int _endTime;
		public float _price;
		public bool _up;

		public Bet(int startTime, int endTime, float price, bool up)
		{
			_startTime = startTime;
			_endTime = endTime;
			_price = price;
			_up = up;
		}
	}
}

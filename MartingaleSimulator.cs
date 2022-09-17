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
				for (int i = 0; i < 999999; i++)
				{
					MessageBox.Show($"{i}");
				}
			}
		}
	}
}

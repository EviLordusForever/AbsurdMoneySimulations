﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class Core
	{
		public static void OnAppStarting()
		{
			FormsManager.mainForm = new MainForm();
			FormsManager.mainForm.Show();
			Logger.Log("I am starting...");
			FormsManager.mainForm.BringToFront();
			Logger.Log("Hello my dear!");
		}

		public static void StartEvolution()
		{
			Thread myThread = new Thread(StartEvolutionThread);
			myThread.Start();

			void StartEvolutionThread()
			{
				NN.Load();
				NN.Init();

				NNTester.LoadGrafic();
				NNTester.FillTests();

				NN.Evolve();
			}
		}

		public static void StartTrader()
		{
			Thread myThread = new Thread(StartTraderThread);
			myThread.Start();

			void StartTraderThread()
			{
				Trader.DoIt();
			}
		}
	}
}

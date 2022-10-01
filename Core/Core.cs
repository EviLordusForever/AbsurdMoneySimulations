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
			Logger.Log("I am starting...");
			FormsManager.mainForm.BringToFront();
			Logger.Log("Hello my dear!");
		}

		public static void RecreateNN()
		{
			Thread myThread = new Thread(StartEvolutionThread);
			myThread.Start();

			void StartEvolutionThread()
			{
				if (UserAsker.Ask("Are you shure?"))
				{
					NN.Create();
					NN.Save();
					NN.Init();
				}
			}
		}

		public static void StartEvolution()
		{
			Thread myThread = new Thread(StartEvolutionThread);
			myThread.Start();

			void StartEvolutionThread()
			{
				NN.Load();
				NN.Init();

				NNTester.InitForEvolution();

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

		public static void TF()
		{
			Thread myThread = new Thread(TFThread);
			myThread.Start();

			void TFThread()
			{
				AbsurdMoneySimulations.TF.LetsGo();
			}
		}
	}
}

﻿namespace AbsurdMoneySimulations
{
	public static class Core
	{
		public static void OnAppStarting()
		{
			Logger.Log("I am starting...");
			FormsManager._mainForm.BringToFront();
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
					NN.InitTesters();
					NN.Create();
					NN.Save();
					NN.Init();
				}
			}
		}

		public static void StartEvolutionByRandomMutations()
		{
			Thread myThread = new Thread(StartEvolutionThread);
			myThread.Name = "EVOLUTION thread";
			myThread.Start();

			void StartEvolutionThread()
			{
				NN.Load();
				NN.Init();

				NN.EvolveByRandomMutations();
			}
		}

		public static void StartNeuralBattle()
		{
			Thread myThread = new Thread(NeuralBattleThread);
			myThread.Name = "Neural Battle Thread";
			myThread.Start();

			void NeuralBattleThread()
			{
				if (UserAsker.Ask("Are you shure?"))
					NN.NeuralBattle();
			}
		}

		public static void StartEvolutionByBackPropgation()
		{
			Thread myThread = new Thread(StartEvolutionThread);
			myThread.Name = "EVOLUTION thread";
			myThread.Start();

			void StartEvolutionThread()
			{
				NN.Load();
				NN.Init();

				NN.EvolveByBackPropagtion();
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

		public static void StartROI()
		{
			Thread myThread = new Thread(StartTraderThread);
			myThread.Name = "ROI";
			myThread.Start();

			void StartTraderThread()
			{
				ROI.LetsDoIt();
			}
		}
	}
}

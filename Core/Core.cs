using System;
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
	}
}

namespace AbsurdMoneySimulations
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
			Thread myThread = new Thread(RecreateNNThread);
			myThread.Start();

			void RecreateNNThread()
			{
				if (UserAsker.Ask("Are you shure?"))
					Manager.RecreateNN();
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
					Manager.NeuralBattle();
			}
		}

		public static void StartEvolutionByBackPropgation()
		{
			Thread myThread = new Thread(StartEvolutionThread);
			myThread.Name = "EVOLUTION thread";
			myThread.Start();

			void StartEvolutionThread()
			{
				Manager.EvolveByBackPropagation();
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

		public static void StartSwarm()
		{
			Thread myThread = new Thread(StartSwarmThread);
			myThread.Name = "Swarm";
			myThread.Start();

			void StartSwarmThread()
			{
				Swarm.CalculateSwarmStatistics();
			}
		}

		public static void StartFindDetailedSectionsStatistics()
		{
			Thread myThread = new Thread(StartFindDetailedSectionsStatisticsThread);
			myThread.Name = "Det.Stat.Thread";
			myThread.Start();

			void StartFindDetailedSectionsStatisticsThread()
			{
				Manager.FindDetailedSectionsStatistics();
			}
		}

		public static void StartDrawOtcIndicators()
		{
			Thread myThread = new Thread(OTC);
			myThread.Name = "OTC indicators drawer";
			myThread.Start();

			void OTC()
			{
				DrawerOfOtcIndicators drawer = new DrawerOfOtcIndicators();
				NN nn = NN.Load();
				nn._testerE.FillTestsFromOriginalGrafic();
				
				drawer.Draw(nn._testerE.OriginalGrafic);
			}
		}

		public static void StartSwarmEvolution()
		{
			Thread myThread = new Thread(SwarmEvolutionThread);
			myThread.Name = "Swarm Evolution Thread";
			myThread.Start();

			void SwarmEvolutionThread()
			{
				Swarm.EvolveSwarm();
			}
		}
	}
}

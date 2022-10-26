namespace AbsurdMoneySimulations
{
	public static class Starter
	{
		public static void StartRecreatingNN()
		{
			Thread myThread = new Thread(RecreateNNThread);
			myThread.Name = "Reacreating NN Thread";
			myThread.Start();

			void RecreateNNThread()
			{
				if (UserAsker.Ask("Are you shure? Previous neural network will be deleted"))
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
			myThread.Name = "Trader Thread";
			myThread.Start();

			void StartTraderThread()
			{
				Trader.Trade();
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

		public static void StartRecreatingSwarm()
		{
			Thread myThread = new Thread(SwarmRecreatingThread);
			myThread.Name = "Swarm Recreating Thread";
			myThread.Start();

			void SwarmRecreatingThread()
			{
				if (UserAsker.Ask("Are you shure? Previous swarm will be deleted"))
					Swarm.RecreateSwarm();
			}
		}

		public static void StartLiveGraphGetting()
		{
			Thread myThread = new Thread(LiveGraphGettingThread);
			myThread.Name = "Live Graph Getting Thread";
			myThread.Start();

			void LiveGraphGettingThread()
			{
				Trader.GetGraphLive(1);
			}
		}
	}
}

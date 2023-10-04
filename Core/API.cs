using Library;

namespace AbsurdMoneySimulations
{
	public static class API
	{
		public static void RecreateNN()
		{
			Thread myThread = new Thread(RecreateNNThread);
			myThread.Name = "Reacreating NN Thread";
			myThread.Start();

			void RecreateNNThread()
			{
				Manager.RecreateNN();
			}
		}

		public static void NeuralBattle()
		{
			Thread myThread = new Thread(NeuralBattleThread);
			myThread.Name = "Neural Battle Thread";
			myThread.Start();

			void NeuralBattleThread()
			{				
				Manager.NeuralBattle();
			}
		}

		public static void FitNeuralNetwork()
		{
			Thread myThread = new Thread(StartFittingThread);
			myThread.Name = "Fitting thread";
			myThread.Start();

			void StartFittingThread()
			{
				Manager.FitNeuralNetwork();
			}
		}

		public static void TradeByNN()
		{
			LiveGraphGetting();

			Thread myThread = new Thread(StartTraderThread);
			myThread.Name = "Trader Thread";
			myThread.Start();

			void StartTraderThread()
			{
				Trader.TradeByNN();
			}
		}

		public static void TradeBySwarm()
		{
			LiveGraphGetting();

			Thread myThread = new Thread(StartTraderThread);
			myThread.Name = "Trader Thread";
			myThread.Start();

			void StartTraderThread()
			{
				Trader.TradeBySwarm();
			}
		}

		public static void FindSwarmStatistics()
		{
			Thread myThread = new Thread(StartSwarmThread);
			myThread.Name = "Swarm";
			myThread.Start();

			void StartSwarmThread()
			{
				Swarm.CalculateStatistics();
			}
		}

		public static void FindDetailedSectionsStatistics()
		{
			Thread myThread = new Thread(StartFindDetailedSectionsStatisticsThread);
			myThread.Name = "Det.Stat.Thread";
			myThread.Start();

			void StartFindDetailedSectionsStatisticsThread()
			{
				Manager.FindDetailedSectionsStatistics();
			}
		}

		public static void DrawOtcIndicators()
		{
			Thread myThread = new Thread(OTC);
			myThread.Name = "OTC indicators drawer";
			myThread.Start();

			void OTC()
			{
				Manager.DrawOtcIndicators();
			}
		}

		public static void FitSwarm()
		{
			Thread myThread = new Thread(Swarm.Fit);
			myThread.Name = "Swarm Fitting Thread";
			myThread.Start();
		}

		public static void RecreateSwarm()
		{
			Thread myThread = new Thread(SwarmRecreatingThread);
			myThread.Name = "Swarm Recreating Thread";
			myThread.Start();

			void SwarmRecreatingThread()
			{
				Swarm.Recreate();
			}
		}

		public static void SetQtxLoginPassword(string login, string password)
		{
			Disk2.WriteToProgramFiles("qtxLogin", "txt", login, false);
			Disk2.WriteToProgramFiles("qtxPassword", "txt", password, false);
		}

		public static string GetQtxLogin()
		{
			return Disk2.ReadFromProgramFilesTxt("qtxLogin");
		}

		public static string GetQtxPassword()
		{
			return Disk2.ReadFromProgramFilesTxt("qtxPassword");
		}

		public static void LiveGraphGetting()
		{
			Thread myThread = new Thread(LiveGraphGettingThread);
			myThread.Name = "Live Graph Getting Thread";
			myThread.Start();

			void LiveGraphGettingThread()
			{
				Trader.GetGraphLive(2);
			}
		}

		public static void DeleteCookies()
		{
			Thread myThread = new Thread(DeletingCookiesThread);
			myThread.Name = "Deleting Cookies Thread";
			myThread.Start();

			void DeletingCookiesThread()
			{
				Manager.DeleteCookies();
			}
		}

		public static void OpenQtxOnly()
		{
			Thread myThread = new Thread(DeletingCookiesThread);
			myThread.Name = "Deleting Cookies Thread";
			myThread.Start();

			void DeletingCookiesThread()
			{
				Manager.OpenQtxOnly();
			}
		}
	}
}

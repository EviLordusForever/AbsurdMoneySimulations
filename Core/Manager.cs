using static AbsurdMoneySimulations.Logger;
using Library;

namespace AbsurdMoneySimulations
{
	public static class Manager
	{
		public static void NeuralBattle()
		{
			if (UserAsker.Ask("Are you shure? Neural Battle can replace previous network (if it's bad only)"))
			{
				Thread myThread = new Thread(SoThread);
				myThread.Start();

				void SoThread()
				{
					NN nn = NN.Load();

					float record = nn.FindLossSquared(nn._testerE, false);
					Log($"record {record}");
					var files = Directory.GetFiles(Library.Disk2._programFiles + "NN");

					for (int n = 0; ; n++)
					{
						nn = Builder.CreateBasicGoodPerceptron();

						float er = nn.FindLossSquared(nn._testerE, false);
						Log($"er {er}");

						if (er < record)
						{
							Log("This is better!");
							record = er;
							NN.Save(nn);
						}
						else
							Log("This is not better!");
					}
				}
			}
		}

		public static void RecreateNN()
		{
			if (UserAsker.Ask("Are you shure? Previous neural network will be deleted"))
			{
				ClearPreviousNNHistory();
				NN nn = Builder.CreateBasicGoodPerceptron();
				NN.Save(nn);
			}
		}

		public static void ClearPreviousNNHistory()
		{
			Disk2.DeleteFileFromProgramFiles("EvolveHistory.csv");
			Disk2.DeleteFileFromProgramFiles("weights.csv");
			Disk2.ClearDirectory(Disk2._programFiles + "NN\\EarlyStopping");
		}

		public static void EvolveByBackPropagation()
		{
			NN nn = NN.Load();
			nn._LEARNING_RATE = 0.002f;

			nn.FitByBackPropagtion();
		}

		public static void FindDetailedSectionsStatistics()
		{
			NN nn = NN.Load();
			Statistics.CalculateStatistics(nn, nn._testerE);
			Statistics.FindDetailedSectionsStatistics(nn._testerE, "Training");
			Statistics.CalculateStatistics(nn, nn._testerV);
			Statistics.FindDetailedSectionsStatistics(nn._testerV, "Validation");
		}

		public static void DeleteCookies()
		{
			if (BrowserManager._driver == null)
			{
				UserAsker.SayWait("Can't delete cookies: Browser is closed. Can't get domain");
				return;
			}

			string domain = BrowserManager.GetDomain();

			if (UserAsker.Ask("Delete cookies for domain \"{domain}\""))
				BrowserManager.DeleteCookies();
		}

		public static void DrawOtcIndicators()
		{
			DrawerOfOtcIndicators drawer = new DrawerOfOtcIndicators();
			NN nn = NN.Load();
			nn._testerE.FillTestsFromOriginalGraph();

			drawer.Draw(nn._testerE.OriginalGraph);
			Log("drawed");
		}

		public static void OpenQtxOnly()
		{
			BrowserManager.LoadBrowser("https://google.com");
			BrowserManager.LoadCookies();
			Trader.CloseChromeMessage();
			Trader.OpenQtx();
		}
	}
}

using static AbsurdMoneySimulations.Logger;
using Library;

namespace AbsurdMoneySimulations
{
	public static class Manager
	{
		public static void NeuralBattle()
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
					nn = Builder.CreateBasicNN();

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

		public static void RecreateNN()
		{
			Disk2.DeleteFileFromProgramFiles("EvolveHistory.csv");
			Disk2.DeleteFileFromProgramFiles("weights.csv");
			Disk2.ClearDirectory(Disk2._programFiles + "NN\\EarlyStopping");

			NN nn = Builder.CreateBasicNN();
			NN.Save(nn);
		}

		public static void EvolveByBackPropagation()
		{
			NN nn = NN.Load();
			nn._LEARNING_RATE = 0.002f;

			nn.EvolveByBackPropagtion();
		}

		public static void FindDetailedSectionsStatistics()
		{
			NN nn = NN.Load();
			Statistics.CalculateStatistics(nn, nn._testerE);
			Statistics.FindDetailedSectionsStatistics(nn._testerE, "Training");
			Statistics.CalculateStatistics(nn, nn._testerV);
			Statistics.FindDetailedSectionsStatistics(nn._testerV, "Validation");
		}
	}
}

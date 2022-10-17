using static AbsurdMoneySimulations.Logger;

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

				float record = nn.FindErrorRateSquared(nn._testerE);
				Log($"record {record}");
				var files = Directory.GetFiles(Disk._programFiles + "NN");

				for (int n = 0; ; n++)
				{
					nn = NN.CreateBasicNN();

					float er = nn.FindErrorRateSquared(nn._testerE);
					Log($"er {er}");

					if (er < record)
					{
						Log("Эта лучше!");
						record = er;
						NN.Save(nn);
					}
					else
						Log("Эта не лучше!");
				}
			}
		}

		public static void RecreateNN()
		{
			Disk.DeleteFileFromProgramFiles("EvolveHistory.csv");
			Disk.DeleteFileFromProgramFiles("weights.csv");
			Disk.ClearDirectory(Disk._programFiles + "NN\\EarlyStopping");

			NN nn = NN.CreateBasicNN();
			NN.Save(nn);
		}

		public static void EvolveByBackPropagation()
		{
			NN nn = NN.Load();

			nn.EvolveByBackPropagtion();
		}

		public static void EvolveByRandomMutations()
		{
			NN nn = NN.Load();

			nn.EvolveByRandomMutations();
		}
	}
}

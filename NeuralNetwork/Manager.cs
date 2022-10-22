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

				float record = nn.FindLossSquared(nn._testerE, false);
				Log($"record {record}");
				var files = Directory.GetFiles(Library.Disk2._programFiles + "NN");

				for (int n = 0; ; n++)
				{
					nn = NN.CreateBasicNN();

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
			Library.Disk2.DeleteFileFromProgramFiles("EvolveHistory.csv");
			Library.Disk2.DeleteFileFromProgramFiles("weights.csv");
			Library.Disk2.ClearDirectory(Library.Disk2._programFiles + "NN\\EarlyStopping");

			NN nn = NN.CreateBasicNN();
			NN.Save(nn);
		}

		public static void EvolveByBackPropagation()
		{
			NN nn = NN.Load();
			nn._LEARNING_RATE = 0.002f;

			nn.EvolveByBackPropagtion();
		}
	}
}

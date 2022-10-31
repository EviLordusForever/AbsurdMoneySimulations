using static AbsurdMoneySimulations.Logger;
using Library;

namespace AbsurdMoneySimulations
{
	public static class Manager
	{
		public static void OnAppStarting()
		{
			Thread myThread = new Thread(StartingThread);
			myThread.Name = "Starting Thread";
			myThread.Start();

			void StartingThread()
			{
				FormsManager.HideForm(FormsManager._mainForm);

				Disk2.Init();

				Log("So app is starting...");
				FormsManager.HideForm(FormsManager._logForm);

				if (UserHasAccess())
				{
					FormsManager.UnhideForm(FormsManager._logForm);
					FormsManager.UnhideForm(FormsManager._mainForm);
					FormsManager.BringToFrontForm(FormsManager._mainForm);
					Log("Hello my dear!");
				}
				else
				{
					FormsManager.OpenShowForm("U ARE CRINGE SO U ARE BANNED");
					FormsManager.SetShowFormSize(600, 600);
					FormsManager.MoveFormToCenter(FormsManager._showForm);

					Thread showImagesThread = new Thread(ShowImagesThread);
					showImagesThread.Start();

					Thread.Sleep(120000);
					UserAsker.SayWait("Sorry dear, u have no access!");
					FormsManager.CloseForm(FormsManager._mainForm);
				}

				void ShowImagesThread()
				{
					string[] files = Disk2.GetFilesFromProgramFiles("Images\\Ban");

					while (true)
					{
						int i = Math2.rnd.Next(files.Length);
						Image bmp = Bitmap.FromFile(files[i]);
						bmp.RotateFlip(Graphics2.RotateFlipTypeRandom);
						FormsManager.ShowImage(bmp);
						Thread.Sleep(150);
					}
				}
			}
		}

		public static bool UserHasAccess()
		{
			return true;
		}

		public static void NeuralBattle()
		{
			if (UserAsker.Ask("Are you shure? Neural Battle can replace previous network (if it's bad only)"))
			{
				Thread myThread = new Thread(SoThread);
				myThread.Start();

				void SoThread()
				{
					NN nn = NN.Load();

					float record = nn.FindLossSquared(nn._testerT, false);
					Log($"record {record}");
					var files = Directory.GetFiles(Library.Disk2._programFiles + "NN");

					for (int n = 0; ; n++)
					{
						nn = Builder.CreateBasicGoodPerceptron();

						float er = nn.FindLossSquared(nn._testerT, false);
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
				NN nn = Builder.CreateBasicGoodPerceptronDerivative();
				NN.Save(nn);
			}
		}

		public static void ClearPreviousNNHistory()
		{
			Disk2.DeleteFileFromProgramFiles("FittingHistory.csv");
			Disk2.DeleteFileFromProgramFiles("FittingHistory (Version for speedup).csv");
			Disk2.DeleteFileFromProgramFiles("weights.csv");
			Disk2.ClearDirectory(Disk2._programFiles + "NN\\EarlyStopping");
		}

		public static void FitNeuralNetwork()
		{
			NN nn = NN.Load();
			FittingParams ft1 = nn.GetFittingParams();
			FittingParams ft2 = FormsManager.AskFittingParams(ft1);
			if (ft1 != ft2)
			{
				nn.SetFittingParams(ft2);
				NN.Save(nn);
			}
			nn.Fit();
		}

		public static void FindDetailedSectionsStatistics()
		{
			NN nn = NN.Load();
			Statistics.CalculateStatistics(nn, nn._testerT);
			Statistics.FindDetailedSectionsStatistics(nn._testerT, "Training");
			Statistics.CalculateStatistics(nn, nn._testerV);
			Statistics.FindDetailedSectionsStatistics(nn._testerV, "Validation");
			Log("All done");
		}

		public static void DeleteCookies()
		{
			if (Browser._driver == null)
			{
				UserAsker.SayWait("Can't delete cookies: Browser is closed. Can't get domain");
				return;
			}

			string domain = Browser.GetDomain();

			if (UserAsker.Ask("Delete cookies for domain \"{domain}\""))
				Browser.DeleteCookies();
		}

		public static void DrawOtcIndicators()
		{
			DrawerOfOtcIndicators drawer = new DrawerOfOtcIndicators();
			NN nn = NN.Load();
			nn._testerT.FillTests(0);

			drawer.Draw(nn._testerT.OriginalGraph);
			Log("drawed");
		}

		public static void OpenQtxOnly()
		{
			Browser.Load("https://google.com");
			Browser.LoadCookies();
			Trader.CloseChromeMessage();
			Trader.OpenQtx();
		}
	}
}

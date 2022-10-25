using Library;

namespace AbsurdMoneySimulations
{
	public static class Core
	{
		public static void OnAppStarting()
		{
			Thread myThread = new Thread(StartingThread);
			myThread.Name = "Starting Thread";
			myThread.Start();

			void StartingThread()
			{
				Logger.Log("So app is starting...");
				FormsManager.HideForm(FormsManager._mainForm);
				FormsManager.HideForm(FormsManager._logForm);

				if (UserHasAccess())
				{
					FormsManager.UnhideForm(FormsManager._logForm);
					FormsManager.UnhideForm(FormsManager._mainForm);
					FormsManager.BringToFrontForm(FormsManager._mainForm);
					Logger.Log("Hello my dear!");
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
	}
}

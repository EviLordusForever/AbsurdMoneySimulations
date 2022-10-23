using Library;
using static AbsurdMoneySimulations.Logger;

namespace AbsurdMoneySimulations
{
	public static class Swarm
	{
		public static void CalculateSwarmStatistics()
		{
			string[] files = Directory.GetFiles(Disk2._programFiles + "NN\\Swarm");
			NN nn = NN.Load();
			
			Tester tester = nn._testerV;
			float[,] predictions = new float[tester._testsCount, files.Length];
			string csv = "";

			CalculateAllPredictions();

			for (int test = 0; test < tester._testsCount; test++)
			{
				WriteAnswer(test);
				WritePredictions(test);
				WriteAveragePrediction(test);
				WriteResultsForPredictions(test);
				WriteCommonResultsHard(test);
				WriteCommonResultsSoft(test);
				WriteCommonResultSimilar(test);
				WriteCommonResultsSimilarAndCutted(test, 0.31f);
				csv += "\r\n";				
			}

			LogStatisticsForCutter(0, 0.8f, 0.01f);
			LogStatisticsForCutter2(0, 5f, 0.001f);

			Disk2.WriteToProgramFiles("Swarm Statistics", "csv", csv, false);
			Log("\"Swarm Statistics\" saved to csv");

			void CalculateAllPredictions()
			{
				for (int n = 0; n < files.Length; n++)
				{
					nn = NN.Load(files[n]);

					for (int test = 0; test < tester._testsCount; test++)
						predictions[test, n] = nn.Calculate(test, nn._testerV._tests[test], false);
				}
			}

			void WriteAnswer(int test)
			{
				csv += "correct answers,";
				csv += tester._answers[test] + ",";
			}

			void WritePredictions(int test)
			{
				csv += "predictions,";
				for (int n = 0; n < files.Length; n++)
					csv += predictions[test, n] + ",";
			}

			void WriteAveragePrediction(int test)
			{
				csv += "average prediction,";
				float p = 0;
				for (int n = 0; n < files.Length; n++)
					p += predictions[test, n];
				csv += p + ",";
			}

			void WriteResultsForPredictions(int test)
			{
				csv += "results,";
				for (int n = 0; n < files.Length; n++)
					if (predictions[test, n] > 0 && tester._answers[test] > 0 ||
						predictions[test, n] < 0 && tester._answers[test] < 0)
						csv += "1,";
					else
						csv += "0,";
			}

			void WriteCommonResultsHard(int test)
			{
				csv += "does all win,";

				int summ = 0;

				for (int n = 0; n < files.Length; n++)
					if (predictions[test, n] > 0 && tester._answers[test] > 0 ||
						predictions[test, n] < 0 && tester._answers[test] < 0)
						summ++;
					else
						summ--;

				if (summ == files.Length)
					csv += "1,";
				else
					csv += "0,";
			}

			void WriteCommonResultsSoft(int test)
			{
				csv += "does predictions summ win,";
				float summ = 0;
				for (int n = 0; n < files.Length; n++)
					summ += predictions[test, n];

				if (summ > 0 && tester._answers[test] > 0 ||
					summ < 0 && tester._answers[test] < 0)
					csv += ",1,";
				else
					csv += ",-1,";
			}

			void WriteCommonResultSimilar(int test)
			{
				csv += "results when they all agree,";
				bool similar = true;
				for (int n = 1; n < files.Length; n++)
					if (predictions[test, n] > 0 && predictions[test, 0] < 0 ||
						predictions[test, n] < 0 && predictions[test, 0] > 0)
						similar = false;

				if (similar)
				{
					if (tester._answers[test] > 0 && predictions[test, 0] > 0 ||
						tester._answers[test] < 0 && predictions[test, 0] < 0)
						csv += ",1,";
					else
						csv += ",-1,";
				}
				else
					csv += ",0,";
			}

			void WriteCommonResultsSimilarAndCutted(int test, float cutter)
			{
				csv += $"cutted by {cutter},";
				bool isPrediction = true;
				for (int n = 1; n < files.Length; n++)
					if (predictions[test, n] > 0 && predictions[test, 0] < 0 ||
						predictions[test, n] < 0 && predictions[test, 0] > 0)
						isPrediction = false;

				for (int n = 1; n < files.Length; n++)
					if (Math.Abs(predictions[test, n]) < cutter)
						isPrediction = false;

				if (isPrediction)
				{
					if (tester._answers[test] > 0 && predictions[test, 0] > 0 ||
						tester._answers[test] < 0 && predictions[test, 0] < 0)
						csv += ",1,";
					else
						csv += ",-1,";
				}
				else
					csv += ",0,";
			}

			void LogStatisticsForCutter(float start, float end, float step)
			{
				string csv = "";
				for (float cutter = start; cutter <= end; cutter += step)
				{
					float predictionsCount = 0;
					float wins = 0;

					for (int test = 0; test < tester._testsCount; test++)
					{
						bool isPrediction = true;
						for (int nn = 1; nn < files.Length; nn++)
							if (predictions[test, nn] > 0 && predictions[test, 0] < 0 ||
								predictions[test, nn] < 0 && predictions[test, 0] > 0)
								isPrediction = false;

						for (int nn = 1; nn < files.Length; nn++)
							if (Math.Abs(predictions[test, nn]) < cutter)
								isPrediction = false;

						if (isPrediction)
						{
							predictionsCount++;

							if (tester._answers[test] > 0 && predictions[test, 0] > 0 ||
								tester._answers[test] < 0 && predictions[test, 0] < 0)
								wins++;
						}
					}

					//Logger.Log($"d{cutter}: {wins}/{predictionsCount} ({wins / predictionsCount})");
					csv += $"{cutter},{wins},/,{predictionsCount},=,{wins / predictionsCount}\n";
				}
				Disk2.WriteToProgramFiles("Swarm Cutters Statistics 2 (agreement)", "csv", csv, false);
				Log("\"Swarm Cutters Statistics 2 (agreement)\" saved to csv");
			}

			void LogStatisticsForCutter2(float start, float end, float step)
			{
				string csv = "";
				for (float cutter = start; cutter <= end; cutter += step)
				{
					float predictionsCount = 0;
					int wins = 0;

					for (int test = 0; test < tester._testsCount; test++)
					{
						float superPrediction = 0;
						for (int nn = 0; nn < files.Length; nn++)
							superPrediction += predictions[test, nn];

						if (Math.Abs(superPrediction) >= cutter)
						{
							predictionsCount++;

							if (tester._answers[test] > 0 && predictions[test, 0] > 0 ||
								tester._answers[test] < 0 && predictions[test, 0] < 0)
								wins++;
						}
					}

					//Logger.Log($"d{cutter}: {wins}/{predictionsCount} ({wins / predictionsCount})");
					csv += $"{cutter},{wins},/,{predictionsCount},=,{wins / predictionsCount},count,{predictionsCount / tester._testsCount},randomness,{Math2.CalculateRandomness(wins, (int)predictionsCount, 0.5f)}\n";
				}
				Disk2.WriteToProgramFiles("Swarm Cutters Statistics 2 (superPrediciton)", "csv", csv, false);
				Log("\"Swarm Cutters Statistics 2 (superPrediciton)\" saved to csv");
			}
		}

		public static void EvolveSwarm()
		{
			string[] files = Directory.GetFiles(Disk2._programFiles + "NN\\Swarm");

			while (true)
				for (int nnn = 0; nnn < files.Length; nnn++)
				{
					NN nn = NN.Load(files[nnn]);
					nn._LEARNING_RATE = 0.002f;
					nn.EvolveByBackPropagtion(200);

					Thread.Sleep(1000);
					File.Delete(files[nnn]);
					string[] file0 = Directory.GetFiles(Disk2._programFiles + "\\NN");
					File.Copy(file0[0], files[nnn]);
					Log($"Copied swarm nn #{nnn} to swarm");
					Thread.Sleep(1000);
				}
		}
	}
}

using Library;
using static AbsurdMoneySimulations.Logger;

namespace AbsurdMoneySimulations
{
	public static class Swarm
	{
		public static NN[] _swarm;
		public static string[] _files;

		public const int _trainingTestsCount = 20000;
		public const int _batchSize = 2500;
		public const int _validationTestsCount = 8000;
		public const float _LEARNING_RATE = 0.002f;

		public static void CalculateStatistics()
		{
			Log("Starting calculating swarm statistics...");
			Load();

			string[] files = Directory.GetFiles(Disk2._programFiles + "NN\\Swarm");
			
			Tester testerV = _swarm[0]._testerV;
			Tester testerT = _swarm[0]._testerT;
			float[,] predictions;
			float[] predictionsSumms;
			string csv = "";

			predictions = CalculateAllPredictions(testerV);
			predictionsSumms = FindPredictionsSumms(testerV);

/*			for (int test = 0; test < testerV._testsCount; test++)
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
			}*/

			//SaveSwarmStatisticsToCsv();

			SaveBothSidesDetailedStaisticsToCsv(testerV, "VALIDATION");
			SaveSingleSideDetailedStaisticsToCsv(testerV, "VALIDATION");

			predictions = CalculateAllPredictions(testerT);
			predictionsSumms = FindPredictionsSumms(testerT);

			SaveBothSidesDetailedStaisticsToCsv(testerT, "TRAINING");
			SaveSingleSideDetailedStaisticsToCsv(testerT, "TRAINING");

			Log("All done.");


			void SaveSwarmStatisticsToCsv()
			{
				Disk2.WriteToProgramFiles("Swarm Statistics", "csv", csv, false);
				Log("\"Swarm Statistics\" was saved to csv");
			}

			void SaveBothSidesDetailedStaisticsToCsv(Tester tester, string reason)
			{
				csv = Statistics.GetDetailedStatisticsCsv(predictionsSumms, tester, -0.5f, 0.5f, 0.001f, false);
				Disk2.WriteToProgramFiles($"Swarm Cutters Statistics 2 ({reason}) (predictions summ) (Both sides)", "csv", csv, false);
				Log($"Saved to csv \"Swarm Cutters Statistics 2 ({reason}) (predictions summ) (Both sides)\"");
			}

			void SaveSingleSideDetailedStaisticsToCsv(Tester tester, string reason)
			{
				csv = Statistics.GetDetailedStatisticsCsv(predictionsSumms, tester, 0, 0.5f, 0.001f, true);
				Disk2.WriteToProgramFiles($"Swarm Cutters Statistics 2 ({reason}) (predictions summ) (Single side)", "csv", csv, false);
				Log($"Saved to csv \"Swarm Cutters Statistics 2 ({reason}) (predictions summ) (Single side)\"");
			}

			float[] FindPredictionsSumms(Tester tester)
			{
				float[] summs = new float[tester._testsCount];

				for (int test = 0; test < tester._testsCount; test++)
				{
					for (int n = 0; n < files.Length; n++)
						summs[test] += predictions[test, n];

					summs[test] /= files.Length;
				}

				return summs;
			}

			float[] FindSquaredPredictionsSumms(Tester tester)
			{
				float[] summs = new float[tester._testsCount];

				for (int test = 0; test < tester._testsCount; test++)
				{
					for (int n = 0; n < files.Length; n++)
						summs[test] += MathF.Pow(predictions[test, n], 2) * MathF.Sign(predictions[test, n]);

					summs[test] /= tester._testsCount;
					summs[test] = MathF.Sqrt(Math.Abs(summs[test])) * MathF.Sign(summs[test]);
				}

				return summs;
			}

			float[,] CalculateAllPredictions(Tester tester)
			{
				predictions = new float[tester._testsCount, files.Length];

				for (int n = 0; n < files.Length; n++)
					for (int test = 0; test < tester._testsCount; test++)
						predictions[test, n] = _swarm[n].Calculate(test, tester._tests[test], false);

				Log("Predictions are calculated.");
				Log("Now wait...");
				return predictions;
			}

			void WriteAnswer(int test)
			{
				csv += "correct answers,";
				csv += testerV._answers[test] + ",";
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
					if (predictions[test, n] > 0 && testerV._answers[test] > 0 ||
						predictions[test, n] < 0 && testerV._answers[test] < 0)
						csv += "1,";
					else
						csv += "0,";
			}

			void WriteCommonResultsHard(int test)
			{
				csv += "does all win,";

				int summ = 0;

				for (int n = 0; n < files.Length; n++)
					if (predictions[test, n] > 0 && testerV._answers[test] > 0 ||
						predictions[test, n] < 0 && testerV._answers[test] < 0)
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

				if (summ > 0 && testerV._answers[test] > 0 ||
					summ < 0 && testerV._answers[test] < 0)
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
					if (testerV._answers[test] > 0 && predictions[test, 0] > 0 ||
						testerV._answers[test] < 0 && predictions[test, 0] < 0)
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
					if (testerV._answers[test] > 0 && predictions[test, 0] > 0 ||
						testerV._answers[test] < 0 && predictions[test, 0] < 0)
						csv += ",1,";
					else
						csv += ",-1,";
				}
				else
					csv += ",0,";
			}

			void LogAgreementStatisticsForCutters(float start, float end, float step)
			{
				string csv = "";
				for (float cutter = start; cutter <= end; cutter += step)
				{
					float predictionsCount = 0;
					float wins = 0;

					for (int test = 0; test < testerV._testsCount; test++)
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

							if (testerV._answers[test] > 0 && predictions[test, 0] > 0 ||
								testerV._answers[test] < 0 && predictions[test, 0] < 0)
								wins++;
						}
					}

					csv += $"{cutter},{wins},/,{predictionsCount},=,{wins / predictionsCount}\n";
				}
				Disk2.WriteToProgramFiles("Swarm Cutters Statistics 2 (agreement)", "csv", csv, false);
				Log("\"Swarm Cutters Statistics 2 (agreement)\" saved to csv");
			}
		}

		public static void Fit()
		{
			Load();			

			while (true)
				for (int n = 0; n < _swarm.Length; n++)
				{
					_swarm[n].Fit(200, true, false);

					File.Delete(_files[n]);
					NN.Save(_swarm[n], _files[n]);
					Log($"(!!!) SAVED NN #{n} TO SWARM");
				}
		}

		public static void Recreate()
		{
			if (UserAsker.Ask("Are you shure? Previous swarm will be deleted"))
			{
				Disk2.ClearDirectory(Disk2._programFiles + "NN\\Swarm");
				Manager.ClearPreviousNNHistory();

				for (int i = 0; i < 10; i++)
				{
					NN nn = Builder.CreateBasicGoodPerceptron();
					NN.Save(nn, $"{Disk2._programFiles}NN\\Swarm\\Dummy {i + 1}.json");
					Log($"NN {i + 1}");
				}
			}
		}

		public static void Load()
		{
			_files = Directory.GetFiles(Disk2._programFiles + "NN\\Swarm");
			_swarm = new NN[_files.Length];

			for (int n = 0; n < _files.Length; n++)
			{
				Log($"NN {n + 1}");

				_swarm[n] = NN.Load(_files[n]);

				_swarm[n]._LEARNING_RATE = _LEARNING_RATE;

				_swarm[n]._testerT._testsCount = _trainingTestsCount;
				_swarm[n]._testerT._batchSize = _batchSize;
				_swarm[n]._testerT.FillTestsFromHorizonGraph();

				_swarm[n]._testerV._testsCount = _validationTestsCount;
				_swarm[n]._testerV._batchSize = _batchSize;
				_swarm[n]._testerV.FillTestsFromHorizonGraph();

				_swarm[n].InitValues();
			}
		}

		public static float Calculate(float[] input)
		{
			float prediction = 0;
			for (int n = 0; n < _swarm.Length; n++)
				prediction += _swarm[n].Calculate(0, input, false);

			return prediction / _swarm.Length;
		}
	}
}

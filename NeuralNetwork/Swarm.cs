using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace AbsurdMoneySimulations
{
	public static class Swarm
	{
		public static void CalculateSwarmStatistics()
		{
			string[] files = Directory.GetFiles(Disk2._programFiles + "NN\\Swarm");
			NN nn = NN.Load(files[0]);
			
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
				WriteCommonResultsSimilarAndCutted(test, 0.02f);
				csv += "\r\n";				
			}

			LogStatisticsForCutter(0, 0.03f, 0.001f);

			Disk2.WriteToProgramFiles("Swarm test", "csv", csv, false);
			Logger.Log("done");

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

					Logger.Log($"d{cutter}: {wins}/{predictionsCount}");
				}
			}
		}
	}
}

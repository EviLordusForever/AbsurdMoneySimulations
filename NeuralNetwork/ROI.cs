using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class ROI
	{
		public static void LetsDoIt()
		{
			NN nn = NN.Load();

			string[] files = Directory.GetFiles(Disk._programFiles + "NN\\ROI");

			float[,] predictions = new float[nn._testerV._testsCount, files.Length];

			string csv = "";

			for (int n = 0; n < files.Length; n++)
			{
				nn = NN.Load(files[n]);

				for (int test = 0; test < nn._testerV._testsCount; test++)
					predictions[test, n] = nn.Calculate(test, nn._testerV._tests[test]);
			}

			for (int test = 0; test < nn._testerV._testsCount; test++)
			{
				csv += nn._testerV._answers[test] + ",";

				for (int n = 0; n < files.Length; n++)
					csv += predictions[test, n] + ",";

				///////////////////

				int summ = 0;

				for (int n = 0; n < files.Length; n++)
					if (predictions[test, n] > 0 && nn._testerV._answers[test] > 0 || 
						predictions[test, n] < 0 && nn._testerV._answers[test] < 0)
					{
						csv += "1,";
						summ++;
					}
					else
					{
						csv += "0,";
						summ--;
					}

				if (summ == files.Length)
					csv += "1,";
				else
					csv += "0,";


				////////////////////


				float summ2 = 0;
				for (int n = 0; n < files.Length; n++)
					summ2 += predictions[test, n];

				if (summ2 > 0 && nn._testerV._answers[test] > 0 || 
					summ2 < 0 && nn._testerV._answers[test] < 0)
					csv += ",1,";
				else
					csv += ",0,";

				////////////////////////

				bool similar = true;
				for (int n = 1; n < files.Length; n++)
					if (predictions[test, n] > 0 && predictions[test, 0] < 0 ||
						predictions[test, n] < 0 && predictions[test, 0] > 0)
						similar = false;

				if (similar)
				{
					if (nn._testerV._answers[test] > 0 && predictions[test, 0] > 0 ||
						nn._testerV._answers[test] < 0 && predictions[test, 0] < 0)
						csv += ",1,";
					else
						csv += ",-1,";
				}
				else
					csv += ",0,";


				//////////////////////////////


				bool isPrediction = true;
				for (int n = 1; n < files.Length; n++)
					if (predictions[test, n] > 0 && predictions[test, 0] < 0 ||
						predictions[test, n] < 0 && predictions[test, 0] > 0)
						isPrediction = false;

				for (int n = 1; n < files.Length; n++)
					if (Math.Abs(predictions[test, n]) < 0.02f)
						isPrediction = false;

				if (isPrediction)
				{
					if (nn._testerV._answers[test] > 0 && predictions[test, 0] > 0 ||
						nn._testerV._answers[test] < 0 && predictions[test, 0] < 0)
						csv += ",1,";
					else
						csv += ",-1,";
				}
				else
					csv += ",0,";

				///////////////////////////////////////////

				csv += "\r\n";				
			}

			for (float d = 0; d < 0.03; d += 0.001f)
				So(d);

			void So(float d)
			{
				float predictionsCount = 0;
				float wins = 0;

				for (int test = 0; test < nn._testerV._testsCount; test++)
				{
					bool isPrediction = true;
					for (int nn = 1; nn < files.Length; nn++)
						if (predictions[test, nn] > 0 && predictions[test, 0] < 0 ||
							predictions[test, nn] < 0 && predictions[test, 0] > 0)
							isPrediction = false;

					for (int nn = 1; nn < files.Length; nn++)
						if (Math.Abs(predictions[test, nn]) < d)
							isPrediction = false;

					if (isPrediction)
					{
						predictionsCount++;

						if (nn._testerV._answers[test] > 0 && predictions[test, 0] > 0 ||
							nn._testerV._answers[test] < 0 && predictions[test, 0] < 0)
							wins++;
					}
				}

				Logger.Log($"d{d}: {wins}/{predictionsCount}");
			}

			Disk.WriteToProgramFiles("ROI test", "csv", csv, false);
			Logger.Log("done");


		}
	}
}

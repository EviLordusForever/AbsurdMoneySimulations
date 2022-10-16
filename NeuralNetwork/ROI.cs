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
			NN.InitTesters();

			string[] files = Directory.GetFiles(Disk._programFiles + "NN\\ROI");

			float[,] predictions = new float[NN._testerV._testsCount, files.Length];

			string csv = "";

			for (int nn = 0; nn < files.Length; nn++)
			{
				NN.Load(files[nn]);
				NN.Init();
				for (int test = 0; test < NN._testerV._testsCount; test++)
					predictions[test, nn] = NN.Calculate(test, NN._testerV._tests[test]);
			}

			for (int test = 0; test < NN._testerV._testsCount; test++)
			{
				csv += NN._testerV._answers[test] + ",";

				for (int nn = 0; nn < files.Length; nn++)
					csv += predictions[test, nn] + ",";

				///////////////////

				int summ = 0;

				for (int nn = 0; nn < files.Length; nn++)
					if (predictions[test, nn] > 0 && NN._testerV._answers[test] > 0 || predictions[test, nn] < 0 && NN._testerV._answers[test] < 0)
					{
						csv += "1,";
						summ++;
					}
					else
					{
						csv += "0,";
						summ--;
					}

				////////////////////

				if (summ == files.Length)
					csv += "1,";
				else
					csv += "0,";

				float summ2 = 0;
				for (int nn = 0; nn < files.Length; nn++)
					summ2 += predictions[test, nn];

				if (summ2 > 0 && NN._testerV._answers[test] > 0 || summ2 < 0 && NN._testerV._answers[test] < 0)
					csv += ",1,";
				else
					csv += ",0,";

				////////////////////////

				bool similar = true;
				for (int nn = 1; nn < files.Length; nn++)
					if (predictions[test, nn] > 0 && predictions[test, 0] < 0 ||
						predictions[test, nn] < 0 && predictions[test, 0] > 0)
						similar = false;

				if (similar)
				{
					if (NN._testerV._answers[test] > 0 && predictions[test, 0] > 0 ||
						NN._testerV._answers[test] < 0 && predictions[test, 0] < 0)
						csv += ",1,";
					else
						csv += ",-1,";
				}
				else
					csv += ",0,";

				csv += "\r\n";
			}

			Disk.WriteToProgramFiles("ROI test", "csv", csv, false);
			Logger.Log("done");
		}
	}
}

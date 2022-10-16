using static AbsurdMoneySimulations.Logger;

namespace AbsurdMoneySimulations
{
	public class Tester
	{
		public int _testsCount;
		public int _batchesCount;
		public int _batchSize;

		private float[] _originalGrafic;
		private float[] _derivativeOfGrafic;
		private float[] _normalizedDerivativeOfGrafic; //[-1, 1]

		public List<int> _availableGraficPoints;
		public float[][] _tests;
		public float[] _answers;
		public byte[] _batch;

		public void InitFromNormalizedOriginalGrafic(string path, string reason)
		{
			LoadOrigGrafic(path, reason);
			FillDerivativeOfGrafic();
			NormalizeDerivativeOfGrafic();

			FillTestsFromOriginalGrafic();
		}

		public void InitFromNormalizedDerivativeGrafic(string path, string reason)
		{
			LoadOrigGrafic(path, reason);
			FillDerivativeOfGrafic();
			NormalizeDerivativeOfGrafic();

			FillTestsFromNormilizedDerivativeGrafic();
		}

		private void LoadOrigGrafic(string graficFolder, string reason)
		{
			var files = Directory.GetFiles(Disk._programFiles + graficFolder);
			var graficL = new List<float>();
			_availableGraficPoints = new List<int>();

			int g = 0;

			for (int f = 0; f < files.Length; f++)
			{
				string[] lines = File.ReadAllLines(files[f]);

				int l = 0;
				while (l < lines.Length)
				{
					graficL.Add(Convert.ToSingle(lines[l]));

					if (l < lines.Length - NN._inputWindow - NN._horizon - 2)
						_availableGraficPoints.Add(g);

					l++; g++;
				}

				Log($"Loaded grafic: \"{TextMethods.StringInsideLast(files[f], "\\", ".")}\"");
			}

			_originalGrafic = graficL.ToArray();
			Log($"Original (and discrete) grafic for {reason} loaded.");
			Log("Also available grafic points are loaded.");
			Log("Grafic length: " + _originalGrafic.Length + ".");
		}

		private void FillDerivativeOfGrafic()
		{
			_derivativeOfGrafic = new float[_originalGrafic.Length];
			for (int i = 1; i < _originalGrafic.Length; i++)
				_derivativeOfGrafic[i] = _originalGrafic[i] - _originalGrafic[i - 1];
			Log("Derivative of grafic is filled.");
		}

		private void NormalizeDerivativeOfGrafic()
		{
			_normalizedDerivativeOfGrafic = new float[_originalGrafic.Length];
			ActivationFunction af = new TanH();
			for (int i = 1; i < _derivativeOfGrafic.Length; i++)
				_normalizedDerivativeOfGrafic[i] = af.f(_derivativeOfGrafic[i]);
			Log("Derivative of grafic is normilized.");
		}

		private void FillTestsFromNormilizedDerivativeGrafic()
		{
			ActivationFunction af = new TanH();

			int maximalDelta = _availableGraficPoints.Count();
			float delta_delta = 0.990f * maximalDelta / _testsCount;

			_tests = new float[_testsCount][];
			_answers = new float[_testsCount];

			int test = 0;
			for (float delta = 0; delta < maximalDelta && test < _testsCount; delta += delta_delta)
			{
				int offset = _availableGraficPoints[Convert.ToInt32(delta)];

				_tests[test] = Extensions.SubArray(_normalizedDerivativeOfGrafic, offset, NN._inputWindow);

				float[] ar = Extensions.SubArray(_derivativeOfGrafic, offset + NN._inputWindow, NN._horizon);
				for (int j = 0; j < ar.Length; j++)
					_answers[test] += ar[j];

				_answers[test] = af.f(_answers[test]);

				test++;
			}

			Log($"Tests and answers for NN are filled from NORMILIZED DERIVATIVE grafic. ({_tests.Length})");
		}

		private void FillTestsFromOriginalGrafic()
		{
			int maximalDelta = _availableGraficPoints.Count();
			float delta_delta = 0.990f * maximalDelta / _testsCount;

			_tests = new float[_testsCount][];
			_answers = new float[_testsCount];

			int test = 0;
			for (float delta = 0; delta < maximalDelta && test < _testsCount; delta += delta_delta)
			{
				int offset = _availableGraficPoints[Convert.ToInt32(delta)];

				_tests[test] = Extensions.SubArray(_originalGrafic, offset, NN._inputWindow);
				Normalize(test);

				float[] ar = Extensions.SubArray(_derivativeOfGrafic, offset + NN._inputWindow, NN._horizon);
				for (int j = 0; j < ar.Length; j++)
					_answers[test] += ar[j];

				_answers[test] = NN._answersAF.f(_answers[test]);

				test++;
			}

			Log($"Tests and answers for NN are filled from NORMILIZED ORIGINAL grafic. ({_tests.Length})");

			void Normalize(int test)
			{
				float min = Extensions.Min(_tests[test]);
				float max = Extensions.Max(_tests[test]);
				float scale = max - min;

				for (int i = 0; i < _tests[test].Length; i++)
					//_tests[test][i] = 2 * (_tests[test][i] - min) / scale - 1;
					_tests[test][i] = (_tests[test][i] - min) / scale;
			}
		}

		public void FillBatch()
		{
			if (_batchesCount > 1)
			{
				_batch = new byte[_testsCount];

				int i = 0;
				while (i < _batchSize)
				{
					int n = Storage.rnd.Next(_testsCount);
					if (_batch[n] == 0)
					{
						_batch[n] = 1;
						i++;
					}
				}
			}
		}

		public void FillBatchBy(int count)
		{
			_batch = new byte[_testsCount];

			int i = 0;
			while (i < count)
			{
				int n = Storage.rnd.Next(_testsCount);
				if (_batch[n] == 0)
				{
					_batch[n] = 1;
					i++;
				}
			}
		}

		private void FillFullBatch()
		{
			for (int i = 0; i < _testsCount; i++)
				_batch[i] = 1;
		}

		public float[] OriginalGrafic
		{
			get
			{
				return _originalGrafic;
			}
		}

		public Tester(int testsCount, int batchesCount, string graficPath, string reason)
		{
			_testsCount = testsCount;
			_batch = new byte[testsCount];
			_batchesCount = batchesCount;
			_batchSize = testsCount / batchesCount;

			//InitFromNormalizedOriginalGrafic(graficPath, reason);
			InitFromNormalizedDerivativeGrafic(graficPath, reason);
			if (batchesCount == 1)
				FillFullBatch();
		}
	}
}

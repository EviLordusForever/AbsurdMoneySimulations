﻿using static AbsurdMoneySimulations.Logger;
using Newtonsoft.Json;

namespace AbsurdMoneySimulations
{
	public class Tester
	{
		public const int _graficJumpLimit = 9000;

		public int _testsCount;
		public int _batchesCount;
		public int _batchSize;

		[JsonIgnore] private float[] _originalGrafic;
		[JsonIgnore] private float[] _derivativeOfGrafic;
		[JsonIgnore] private float[] _normalizedDerivativeOfGrafic; //[-1, 1]

		[JsonIgnore] public List<int> _availableGraficPoints;
		[JsonIgnore] public float[][] _tests;
		[JsonIgnore] public float[] _answers;
		[JsonIgnore] public byte[] _batch;

		[JsonIgnore] private NN _ownerNN { get; set; }

		private void LoadGrafic(string path, string reason)
		{
			LoadOriginalGrafic(path, reason);
			FillDerivativeOfGrafic();
			NormalizeDerivativeOfGrafic();
		}

		private void LoadOriginalGrafic(string graficFolder, string reason)
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

					if (l < lines.Length - _ownerNN._inputWindow - _ownerNN._horizon - 2)
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

			for (int i = 1; i < _derivativeOfGrafic.Length; i++)
				_normalizedDerivativeOfGrafic[i] = _ownerNN._inputAF.f(_derivativeOfGrafic[i]);
			Log("Derivative of grafic is normilized.");
		}

		public void FillTestsFromNormilizedDerivativeGrafic()
		{
			int maximalDelta = _availableGraficPoints.Count();
			float delta_delta = 0.990f * maximalDelta / _testsCount;

			_tests = new float[_testsCount][];
			_answers = new float[_testsCount];

			int test = 0;
			for (float delta = 0; delta < maximalDelta && test < _testsCount; delta += delta_delta)
			{
				int offset = _availableGraficPoints[Convert.ToInt32(delta)];

				_tests[test] = Extensions.SubArray(_normalizedDerivativeOfGrafic, offset, _ownerNN._inputWindow);

				float[] ar = Extensions.SubArray(_derivativeOfGrafic, offset + _ownerNN._inputWindow, _ownerNN._horizon);
				for (int j = 0; j < ar.Length; j++)
					_answers[test] += ar[j];

				_answers[test] = _ownerNN._answersAF.f(_answers[test]);

				test++;
			}

			Log($"Tests and answers for NN are filled from NORMILIZED DERIVATIVE grafic. ({_tests.Length})");
		}

		public void FillTestsFromOriginalGrafic()
		{
			int maximalDelta = _availableGraficPoints.Count();
			float delta_delta = 0.990f * maximalDelta / _testsCount;

			_tests = new float[_testsCount][];
			_answers = new float[_testsCount];

			int test = 0;
			for (float delta = 0; delta < maximalDelta && test < _testsCount; delta += delta_delta)
			{
				int offset = _availableGraficPoints[Convert.ToInt32(delta)];

				_tests[test] = Extensions.SubArray(_originalGrafic, offset, _ownerNN._inputWindow);
				Normalize(test);

				float[] ar = Extensions.SubArray(_derivativeOfGrafic, offset + _ownerNN._inputWindow, _ownerNN._horizon);
				for (int j = 0; j < ar.Length; j++)
					_answers[test] += ar[j];

				_answers[test] = _ownerNN._answersAF.f(_answers[test]);

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

		[JsonIgnore]
		public float[] OriginalGrafic
		{
			get
			{
				return _originalGrafic;
			}
		}

		public Tester()
		{
		}

		public Tester(NN ownerNN, int testsCount, int batchesCount, string graficPath, string reason, bool originalOrDerivativeGrafic)
		{
			_ownerNN = ownerNN;
			_testsCount = testsCount;
			_batch = new byte[testsCount];
			_batchesCount = batchesCount;
			_batchSize = testsCount / batchesCount;

			LoadGrafic(graficPath, reason);

			if (originalOrDerivativeGrafic)
				FillTestsFromOriginalGrafic();
			else
				FillTestsFromNormilizedDerivativeGrafic();

			if (batchesCount == 1)
				FillFullBatch();
		}

		public void SetOwnerNN(NN ownerNN)
		{
			_ownerNN = ownerNN;
		}
	}
}

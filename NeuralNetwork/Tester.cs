using static AbsurdMoneySimulations.Logger;
using Newtonsoft.Json;
using Library;

namespace AbsurdMoneySimulations
{
	public class Tester
	{
		public int _testsCount;
		public int _batchSize;

		public int _moveAnswersOverZero;
		public int _moveInputsOverZero;
		public int _graphLoadingType;

		public string _graphPath;
		public string _reason;

		[JsonIgnore] private float[] _originalGraph;
		[JsonIgnore] private float[] _derivativeOfGraph;
		[JsonIgnore] private float[] _normalizedDerivativeOfGraph;
		[JsonIgnore] private float[] _horizonGraph;

		[JsonIgnore] public List<int> _availableGraphPoints;
		[JsonIgnore] public List<int> _availableGraphPointsForHorizonGraph;
		[JsonIgnore] public float[][] _tests;
		[JsonIgnore] public float[] _answers;
		[JsonIgnore] public byte[] _batch;

		[JsonIgnore] private NN _ownerNN { get; set; }

		private void LoadGraph(string path, string reason)
		{
			LoadOriginalGraph(path, reason);
			FillDerivativeOfGraph();
			NormalizeDerivativeOfGraph();
			FillHorizonOfGraph();
		}

		private void LoadOriginalGraph(string graphFolder, string reason)
		{
			var files = Directory.GetFiles(Disk2._programFiles + graphFolder);
			var graphL = new List<float>();
			_availableGraphPoints = new List<int>();
			_availableGraphPointsForHorizonGraph = new List<int>();

			int g = 0;

			for (int f = 0; f < files.Length; f++)
			{
				string[] lines = File.ReadAllLines(files[f]);

				int l = 0;
				while (l < lines.Length)
				{
					graphL.Add(Convert.ToSingle(lines[l]));

					if (l < lines.Length - _ownerNN._inputWindow - _ownerNN._horizon - 2)
					{
						_availableGraphPoints.Add(g);

						if (l > _ownerNN._horizon)
							_availableGraphPointsForHorizonGraph.Add(g);
					}

					l++; g++;
				}

				Log($"Loaded graph: \"{Text2.StringBeforeLast(Text2.StringAfterLast(files[f], "\\"), ".")}\"");
			}

			_originalGraph = graphL.ToArray();
			Log($"Original (and discrete) graph for {reason} loaded.");
			Log("Also available graph points (x2) are loaded.");
			Log("Graph length: " + _originalGraph.Length + ".");
		}

		private void FillDerivativeOfGraph()
		{
			_derivativeOfGraph = new float[_originalGraph.Length];
			for (int i = 1; i < _originalGraph.Length; i++)
				_derivativeOfGraph[i] = _originalGraph[i] - _originalGraph[i - 1];
			Log("Derivative of graph is filled.");
		}

		private void NormalizeDerivativeOfGraph()
		{
			_normalizedDerivativeOfGraph = new float[_originalGraph.Length];

			for (int i = 1; i < _derivativeOfGraph.Length; i++)
				_normalizedDerivativeOfGraph[i] = _ownerNN._inputAF.f(_derivativeOfGraph[i]);
			Log("Derivative of graph is normilized.");
		}

		private void FillHorizonOfGraph()
		{
			_horizonGraph = new float[_originalGraph.Length];
			for (int i = _ownerNN._horizon; i < _originalGraph.Length; i++)
				_horizonGraph[i] = _originalGraph[i] - _originalGraph[i - _ownerNN._horizon];
			Log("HorizonGraph is filled.");
		}

		public void FillTestsFromNormilizedDerivativeGraph()
		{
			int maximalDelta = _availableGraphPoints.Count();
			float delta_delta = 0.990f * maximalDelta / _testsCount;

			_tests = new float[_testsCount][];
			_answers = new float[_testsCount];

			int test = 0;
			for (float delta = 0; delta < maximalDelta && test < _testsCount; delta += delta_delta)
			{
				int offset = _availableGraphPoints[Convert.ToInt32(delta)];

				_tests[test] = Array2.SubArray(_normalizedDerivativeOfGraph, offset, _ownerNN._inputWindow);

				Normalize(test);

				float[] ar = Array2.SubArray(_derivativeOfGraph, offset + _ownerNN._inputWindow, _ownerNN._horizon);
				for (int j = 0; j < ar.Length; j++)
					_answers[test] += ar[j];

				_answers[test] = _ownerNN._answersAF.f(_answers[test]) + _moveAnswersOverZero;

				test++;
			}

			Log($"Tests and answers for NN are filled from NORMILIZED DERIVATIVE graph. ({_tests.Length})");

			void Normalize(int test)
			{
				for (int i = 0; i < _tests[test].Length; i++)
					_tests[test][i] += _moveInputsOverZero;
			}
		}

		public void FillTestsFromOriginalGraph()
		{
			int maximalDelta = _availableGraphPoints.Count();
			float delta_delta = 0.990f * maximalDelta / _testsCount;

			_tests = new float[_testsCount][];
			_answers = new float[_testsCount];

			int test = 0;
			for (float delta = 0; delta < maximalDelta && test < _testsCount; delta += delta_delta)
			{
				int offset = _availableGraphPoints[Convert.ToInt32(delta)];

				_tests[test] = Array2.SubArray(_originalGraph, offset, _ownerNN._inputWindow);
				Normalize(test);

				float[] ar = Array2.SubArray(_derivativeOfGraph, offset + _ownerNN._inputWindow, _ownerNN._horizon);
				for (int j = 0; j < ar.Length; j++)
					_answers[test] += ar[j];

				_answers[test] = _ownerNN._answersAF.f(_answers[test]) + _moveAnswersOverZero;

				test++;
			}

			Log($"Tests and answers for NN are filled from NORMILIZED ORIGINAL graph. ({_tests.Length})");

			void Normalize(int test)
			{				
				float final = _tests[test][_tests[test].Length - 1];

				for (int i = 0; i < _tests[test].Length; i++)
					_tests[test][i] = _tests[test][i] - final;

				float min = Math2.Min(_tests[test]);
				float max = Math2.Max(_tests[test]);

				max = MathF.Max(MathF.Abs(max), MathF.Abs(min));

				for (int i = 0; i < _tests[test].Length; i++)
					_tests[test][i] = _tests[test][i] / max + _moveInputsOverZero;
			}
		}

		public void FillTestsFromHorizonGraph()
		{
			int maximalDelta = _availableGraphPointsForHorizonGraph.Count();
			float delta_delta = 0.990f * maximalDelta / _testsCount;

			_tests = new float[_testsCount][];
			_answers = new float[_testsCount];

			int test = 0;
			for (float delta = 0; delta < maximalDelta && test < _testsCount; delta += delta_delta)
			{
				int offset = _availableGraphPointsForHorizonGraph[Convert.ToInt32(delta)];

				_tests[test] = Array2.SubArray(_horizonGraph, offset, _ownerNN._inputWindow);
				float standartDeviation = Math2.FindStandartDeviation(_tests[test]);
				_tests[test] = Normalize(_tests[test], standartDeviation, _ownerNN._inputAF, _moveInputsOverZero);

				_answers[test] = _horizonGraph[offset + _ownerNN._inputWindow + _ownerNN._horizon];
				_answers[test] = _ownerNN._answersAF.f(_answers[test] / standartDeviation) + _moveAnswersOverZero;

				test++;
			}

			Log($"Tests and answers for NN are filled from NORMALIZED HORIZON(!!!) graph. ({_tests.Length})");
		}

		public static float[] Normalize(float[] input, float standartDeviation, ActivationFunction af, float move)
		{
			for (int i = 0; i < input.Length; i++)
				input[i] = af.f(input[i] / standartDeviation) + move;

			return input;
		}

		public void FillBatch()
		{
			FillBatchBy(_batchSize);
		}

		public void FillBatchBy(int count)
		{
			if (count == _testsCount)
				FillFullBatch();
			else
			{
				_batch = new byte[_testsCount];

				int i = 0;
				while (i < count)
				{
					int n = Math2.rnd.Next(_testsCount);
					if (_batch[n] == 0)
					{
						_batch[n] = 1;
						i++;
					}
				}
			}
		}

		public void FillFullBatch()
		{
			for (int i = 0; i < _testsCount; i++)
				_batch[i] = 1;
		}

		[JsonIgnore]
		public float[] OriginalGraph
		{
			get
			{
				return _originalGraph;
			}
		}

		public void Init(NN ownerNN, string graphPath, string reason)
		{
			_ownerNN = ownerNN;
			_batch = new byte[_testsCount];

			if (graphPath != null)
			{
				LoadGraph(graphPath, reason);

				if (_batchSize == _testsCount)
					FillFullBatch();

				if (_graphLoadingType == 0)
					FillTestsFromOriginalGraph();
				else if (_graphLoadingType == 1)
					FillTestsFromNormilizedDerivativeGraph();
				else if (_graphLoadingType == 2)
					FillTestsFromHorizonGraph();
				else
					throw new Exception();
			}
		}

		public Tester(NN ownerNN, int testsCount, int batchSize, string graphPath, string reason, int graphLoadingType, int moveInputsOverZero, int moveAnswersOverZero)
		{
			_testsCount = testsCount;
			_batchSize = batchSize;

			_moveAnswersOverZero = moveAnswersOverZero;
			_moveInputsOverZero = moveInputsOverZero;
			_graphLoadingType = graphLoadingType;

			_graphPath = graphPath;
			_reason = reason;

			Init(ownerNN, _graphPath, _reason);
		}

		public void SetOwnerNN(NN ownerNN)
		{
			_ownerNN = ownerNN;
		}
	}
}

using Newtonsoft.Json;

namespace AbsurdMoneySimulations
{
	public class Node
	{
		public float[] _weights;

		public float _bias;

		[JsonIgnore]
		public float[][] _subvalues;

		[JsonIgnore]
		public float[] _biasvalues;

		[JsonIgnore]
		public float[] _summ;

		[JsonIgnore]
		public float[] _BPgradient;

		[JsonIgnore]
		private NN _ownerNN { get; set; }

		public int _lastMutatedWeight;

		public void FillRandomly()
		{
			float scale = MathF.Abs(_ownerNN._weightsInitMax - _ownerNN._weightsInitMin);

			for (int i = 0; i < _weights.Count(); i++)
				_weights[i] = Storage.rnd.NextSingle() * scale + _ownerNN._weightsInitMin;
			_bias = 0;
		}

		public float Calculate(int test, float[] input, int start)
		{
			_biasvalues[test] = _bias * _ownerNN._biasInput;
			_summ[test] = _biasvalues[test];

			for (int w = 0; w < _weights.Count(); w++)
			{
				_subvalues[test][w] = _weights[w] * input[start + w];
				_summ[test] += _subvalues[test][w];
			}

			return _summ[test];
		}

		public float CalculateOnlyOneWeight(int test, float input, int w)
		{
			if (w < _weights.Length)
			{
				_summ[test] -= _subvalues[test][w];
				_subvalues[test][w] = _weights[w] * input;
				_summ[test] += _subvalues[test][w];
			}
			else
			{
				_summ[test] -= _biasvalues[test];
				_biasvalues[test] = _bias * _ownerNN._biasInput;
				_summ[test] += _biasvalues[test];
			}

			return _summ[test];
		}

		public void Mutate(float mutagen)
		{
			_lastMutatedWeight = Storage.rnd.Next(_weights.Length + 1);

			if (_lastMutatedWeight == _weights.Length)
				_bias += mutagen;
			else
				_weights[_lastMutatedWeight] += mutagen;
		}

		public void Demutate(float mutagen)
		{
			if (_lastMutatedWeight == _weights.Length)
				_bias -= mutagen;
			else
				_weights[_lastMutatedWeight] -= mutagen;
		}

		public void FindBPGradient(int test, ActivationFunction af, float desiredValue)
		{
			try
			{
				_BPgradient[test] = (af.f(_summ[test]) - desiredValue) * af.df(_summ[test]);
			}
			catch (VanishedGradientException ex)
			{
				_ownerNN._vanishedGradientsCount++;
				_BPgradient[test] = 0;
			}
		}

		public void FindBPGradient(int test, ActivationFunction af, float[] gradients, float[] weights)
		{
			float gwsumm = FindSummOfBPGradientsPerWeights(gradients, weights);
			_BPgradient[test] += gwsumm * af.df(_summ[test]);
			_BPgradient[test] = _ownerNN.CutGradient(_BPgradient[test]);
		}

		public void UseInertionForGradient(int test)
		{
			_BPgradient[test] *= _ownerNN._INERTION;
		}

		public static float FindSummOfBPGradientsPerWeights(float[] gradients, float[] weights)
		{
			float gwsumm = 0;

			for (int gw = 0; gw < gradients.Count(); gw++)
				gwsumm += gradients[gw] * weights[gw];

			return gwsumm;
		}

		public void CorrectWeightsByBP(int test, float[] input, int start)
		{
			if (float.IsNaN(_BPgradient[test]))
			{
			}
			for (int w = 0; w < _weights.Count(); w++)
				_weights[w] -= _ownerNN._LEARNING_RATE * _BPgradient[test] * input[start + w];
			_bias -= _ownerNN._LEARNING_RATE * _BPgradient[test] * _ownerNN._biasInput;
		}

		public Node(NN nn, int testsCount, int weightsCount)
		{
			_ownerNN = nn;

			_weights = new float[weightsCount];

			InitValues(testsCount);
		}

		public void InitValues(int testsCount)
		{
			_summ = new float[testsCount];

			_subvalues = new float[testsCount][];

			for (int test = 0; test < testsCount; test++)
				_subvalues[test] = new float[_weights.Count()];

			_biasvalues = new float[testsCount];

			_BPgradient = new float[testsCount];
		}

		public void SetOwnerNN(NN ownerNN)
		{
			_ownerNN = ownerNN;
		}
	}
}


using Newtonsoft.Json;

namespace AbsurdMoneySimulations
{
	public class Node
	{
		public float[] _weights;

		[JsonIgnore]
		public float[][] _subvalues;

		[JsonIgnore]
		public float[] _summ;

		[JsonIgnore]
		public float[] _BPgradient;

		public int _lastMutatedWeight;

		public void FillRandomly()
		{
			float scale = NN._weightsInitMax - NN._weightsInitMin;
			for (int i = 0; i < _weights.Count(); i++)
				_weights[i] = Storage.rnd.NextSingle() * scale - NN._weightsInitMin;
		}

		public float Calculate(int test, float[] input, int start)
		{
			_summ[test] = 0;

			for (int w = 0; w < _weights.Count(); w++)
			{
				_subvalues[test][w] = _weights[w] * input[start + w];
				_summ[test] += _subvalues[test][w];
			}

			return _summ[test];
		}

		public float CalculateOnlyOneWeight(int test, float input, int w)
		{
			_summ[test] -= _subvalues[test][w];
			_subvalues[test][w] = _weights[w] * input;
			_summ[test] += _subvalues[test][w];

			return _summ[test];
		}

		public void Mutate(float mutagen)
		{
			_lastMutatedWeight = Storage.rnd.Next(_weights.Length);
			_weights[_lastMutatedWeight] += mutagen;
		}

		public void Demutate(float mutagen)
		{
			_weights[_lastMutatedWeight] -= mutagen;
		}

		public void FindBPGradient(int test, ActivationFunction af, float desiredValue)
		{
			_BPgradient[test] = (af.f(_summ[test]) - desiredValue) * af.df(_summ[test]);
		}

		public void FindBPGradient(int test, ActivationFunction af, float[] gradients, float[] weights)
		{
			float gwsumm = FindSummOfBPGradientsPerWeights(gradients, weights);
			_BPgradient[test] += gwsumm * af.df(_summ[test]);
			_BPgradient[test] = NN.CutGradient(_BPgradient[test]);
		}

		public void UseInertionForGradient(int test)
		{
			_BPgradient[test] *= NN._INERTION;
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
				_weights[w] -= NN._LYAMBDA * _BPgradient[test] * input[start + w];
		}

		public Node(int testsCount, int weightsCount)
		{
			_weights = new float[weightsCount];

			InitValues(testsCount);
		}

		public void InitValues(int testsCount)
		{
			_summ = new float[testsCount];

			_subvalues = new float[testsCount][];

			for (int test = 0; test < testsCount; test++)
				_subvalues[test] = new float[_weights.Count()];

			_BPgradient = new float[testsCount];
		}
	}
}


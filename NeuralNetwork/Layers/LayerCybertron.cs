namespace AbsurdMoneySimulations
{
	public class LayerCybertron : Layer
	{
		public LayerPerceptron[] _perceptrons;
		public int _lastMutatedSub;
		public int _outNodesSummCount;

		public override void FillWeightsRandomly()
		{
			for (int i = 0; i < _perceptrons.Count(); i++)
				_perceptrons[i].FillWeightsRandomly();
		}

		public override void Calculate(int test, float[][] input)
		{
			for (int sub = 0; sub < _perceptrons.Length; sub++)
				_perceptrons[sub].Calculate(test, input[sub]);
		}

		public override void Calculate(int test, float[] input)
		{
			throw new NotImplementedException();
		}

		public override LayerRecalculateStatus Recalculate(int test, float[][] input, LayerRecalculateStatus lrs)
		{
			if (lrs == LayerRecalculateStatus.OneWeightChanged)
			{
				_perceptrons[_lastMutatedSub].Recalculate(test, input[_lastMutatedSub], LayerRecalculateStatus.OneWeightChanged);
				lrs._lastMutatedSub = _lastMutatedSub;
				return LayerRecalculateStatus.OneSubChanged;
			}
			else if (lrs == LayerRecalculateStatus.OneSubChanged)
			{
				_perceptrons[_lastMutatedSub].Recalculate(test, input[_lastMutatedSub], LayerRecalculateStatus.Full);
				lrs._lastMutatedSub = _lastMutatedSub;
				return LayerRecalculateStatus.OneSubChanged;
			}
			else
			{
				Calculate(test, input);
				return LayerRecalculateStatus.Full;
			}
		}

		public override void FindBPGradient(int test, float[] innerBPGradients, float[][] innerWeights)
		{
			//very hard to explain without drawing
			for (int n = 0; n < _perceptrons.Count(); n++)
				_perceptrons[n].FindBPGradient(test, innerBPGradients, Extensions.SubArray(innerWeights, n * _perceptrons[0]._nodes.Count(), _perceptrons[0]._nodes.Count()));
		}

		public override void FindBPGradient(int test, float desiredValue)
		{
			throw new NotImplementedException();
		}

		public override void UseInertionForGradient(int test)
		{
			for (int n = 0; n < _perceptrons.Count(); n++)
				_perceptrons[n].UseInertionForGradient(test);
		}

		public override void Mutate(float mutagen)
		{
			_lastMutatedSub = Storage.rnd.Next(_perceptrons.Count());
			_perceptrons[_lastMutatedSub].Mutate(mutagen);
		}

		public override void Demutate(float mutagen)
		{
			_perceptrons[_lastMutatedSub].Demutate(mutagen);
		}

		public override void CorrectWeightsByBP(int test, float[][] input)
		{
			for (int sub = 0; sub < _perceptrons.Length; sub++)
				_perceptrons[sub].CorrectWeightsByBP(test, input[sub]);
		}

		public override float GetAnswer(int test)
		{
			throw new NotImplementedException();
		}

		public override float[][] GetValues(int test)
		{
			int node1 = 0;
			for (int perceptron = 0; perceptron < _perceptrons.Count(); perceptron++)
			{
				for (int node2 = 0; node2 < _perceptrons[perceptron]._nodes.Length;)
				{
					_values[test][0][node1] = _perceptrons[perceptron]._values[test][0][node2];
					node1++;
					node2++;
				}
			}

			return _values[test];
		}

		public override int WeightsCount
		{
			get
			{
				return _perceptrons.Count() * _perceptrons[0].WeightsCount;
			}
		}

		public override float[][] AllWeights
		{
			get
			{
				float[][] weights = _perceptrons[0].AllWeights;

				for (int p = 1; p < _perceptrons.Length; p++)
					weights = Extensions.Concatenate(weights, _perceptrons[p].AllWeights);

				return weights;
				//TEST THIS !!!!!
			}
		}

		public override float[] AllBPGradients(int test)
		{
			float[] gradients = _perceptrons[0].AllBPGradients(test);

			for (int p = 1; p < _perceptrons.Length; p++)
				gradients = Extensions.Concatenate(gradients, _perceptrons[p].AllBPGradients(test));

			return gradients;
			//And this
		}

		public LayerCybertron(int testsCount, int perceptronsCount, int weightsPerNodePerceptronCount, int nodesPerPerceptronCount, int outNodesSummCount, ActivationFunction af)
		{
			_type = "cybertron";

			this._outNodesSummCount = outNodesSummCount;

			_perceptrons = new LayerPerceptron[perceptronsCount];
			for (int p = 0; p < _perceptrons.Count(); p++)
				_perceptrons[p] = new LayerPerceptron(testsCount, nodesPerPerceptronCount, weightsPerNodePerceptronCount, af);

			InitValues(testsCount);
		}

		public override void InitValues(int testsCount)
		{
			_values = new float[testsCount][][];
			for (int test = 0; test < testsCount; test++)
			{
				_values[test] = new float[1][];
				_values[test][0] = new float[_outNodesSummCount];
			}

			for (int p = 0; p < _perceptrons.Count(); p++)
			{
				_perceptrons[p].InitValues(testsCount);
				_perceptrons[p]._af = _af = new TanH();
			}
		}
	}
}

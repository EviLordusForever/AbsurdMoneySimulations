namespace AbsurdMoneySimulations
{
	public class LayerPerceptron : Layer
	{
		public Node[] _nodes;
		public int _lastMutatedNode;

		public override void FillWeightsRandomly()
		{
			for (int i = 0; i < _nodes.Length; i++)
				_nodes[i].FillRandomly();
		}

		public override void Calculate(int test, float[][] input)
		{
			for (int node = 0; node < _nodes.Length; node++)
				_values[test][0][node] = _af.f(_nodes[node].Calculate(test, input[0], 0));
		}

		public override void Calculate(int test, float[] input)
		{
			for (int node = 0; node < _nodes.Length; node++)
				_values[test][0][node] = _af.f(_nodes[node].Calculate(test, input, 0));
		}

		public override LayerRecalculateStatus Recalculate(int test, float[][] input, LayerRecalculateStatus lrs)
		{
			if (lrs == LayerRecalculateStatus.OneWeightChanged)
				return RecalculateAfterOneWeightChanged(test, input);
			else if (lrs == LayerRecalculateStatus.OneNodeChanged)
				return RecalculateAfterOneNodeChanged(test, input, lrs);
			else if (lrs == LayerRecalculateStatus.OneSubChanged)
				return RecalculateAfterOneSubChanged(test, input, lrs);
			else
				return RecalculateAfterEveryNodeChanged(test, input);
		}

		public LayerRecalculateStatus Recalculate(int test, float[] input, LayerRecalculateStatus lrs)
		{
			if (lrs == LayerRecalculateStatus.OneWeightChanged)
				return RecalculateAfterOneWeightChanged(test, input, lrs);
			else
				return RecalculateAfterEveryNodeChanged(test, input);
		}
	
		private LayerRecalculateStatus RecalculateAfterOneWeightChanged(int test, float[] input, LayerRecalculateStatus lrs)
		{
			_values[test][0][_lastMutatedNode] = _af.f(_nodes[_lastMutatedNode].CalculateOnlyOneWeight(test, input[_lastMutatedNode], _nodes[_lastMutatedNode]._lastMutatedWeight));
			lrs = LayerRecalculateStatus.OneNodeChanged;
			lrs._lastMutatedNode = _lastMutatedNode;
			return lrs;
		}

		private LayerRecalculateStatus RecalculateAfterOneWeightChanged(int test, float[][] input)
		{
			_values[test][0][_lastMutatedNode] = _af.f(_nodes[_lastMutatedNode].CalculateOnlyOneWeight(test, input[0][_lastMutatedNode], _nodes[_lastMutatedNode]._lastMutatedWeight));

			LayerRecalculateStatus lrs = LayerRecalculateStatus.OneNodeChanged;
			lrs._lastMutatedNode = _lastMutatedNode;
			return lrs;
		}

		private LayerRecalculateStatus RecalculateAfterOneNodeChanged(int test, float[][] input, LayerRecalculateStatus lrs)
		{
			for (int n = 0; n < _nodes.Length; n++)
				_values[test][0][n] = _af.f(_nodes[n].CalculateOnlyOneWeight(test, input[0][lrs._lastMutatedNode], lrs._lastMutatedNode));
			return LayerRecalculateStatus.Full;
		}

		private LayerRecalculateStatus RecalculateAfterOneSubChanged(int test, float[][] input, LayerRecalculateStatus lrs)
		{
			for (int n = 0; n < _nodes.Length; n++)
			{
				for (int subnode = lrs._lastMutatedSub * lrs._subSize; subnode < lrs._lastMutatedSub * lrs._subSize + lrs._subSize; subnode++)
					_nodes[n].CalculateOnlyOneWeight(test, input[0][subnode], subnode);
				//test me

				_values[test][0][n] = _af.f(_nodes[n]._summ[test]);
			}
			return LayerRecalculateStatus.Full;
		}

		private LayerRecalculateStatus RecalculateAfterEveryNodeChanged(int test, float[][] input)
		{
			Calculate(test, input);
			return LayerRecalculateStatus.Full;
		}

		private LayerRecalculateStatus RecalculateAfterEveryNodeChanged(int test, float[] input)
		{
			Calculate(test, input);
			return LayerRecalculateStatus.Full;
		}

		public override void FindBPGradient(int test, float[] innerBPGradients, float[][] innerWeights)
		{
			for (int n = 0; n < _nodes.Count(); n++)
				_nodes[n].FindBPGradient(test, _af, innerBPGradients, innerWeights[n]);
		}

		public override void FindBPGradient(int test, float desiredValue)
		{
			_nodes[0].FindBPGradient(test, _af, desiredValue);
		}

		public override void UseInertionForGradient(int test)
		{
			for (int n = 0; n < _nodes.Count(); n++)
				_nodes[n].UseInertionForGradient(test);
		}

		public override void Mutate(float mutagen)
		{
			_lastMutatedNode = Storage.rnd.Next(_nodes.Count());
			_nodes[_lastMutatedNode].Mutate(mutagen);
		}

		public override void Demutate(float mutagen)
		{
			_nodes[_lastMutatedNode].Demutate(mutagen);
		}

		public override void CorrectWeightsByBP(int test, float[][] input)
		{
			for (int node = 0; node < _nodes.Length; node++)
				_nodes[node].CorrectWeightsByBP(test, input[0], 0);
		}

		public void CorrectWeightsByBP(int test, float[] input)
		{
			for (int node = 0; node < _nodes.Length; node++)
				_nodes[node].CorrectWeightsByBP(test, input, 0);
		}

		public override float[][] GetValues(int test)
		{
			return _values[test];
		}

		public override float GetAnswer(int test)
		{
			return _af.f(_nodes[0]._summ[test]);
		}

		public override int WeightsCount
		{
			get
			{
				return _nodes.Count() * _nodes[0]._weights.Count();
			}
		}

		public override float[][] AllWeights
		{
			get
			{
				float[][] allWeights = new float[_nodes[0]._weights.Length][];
				for (int weight = 0; weight < _nodes[0]._weights.Length; weight++)
				{
					allWeights[weight] = new float[_nodes.Length];
					for (int node = 0; node < _nodes.Length; node++)
						allWeights[weight][node] = _nodes[node]._weights[weight];
				}
				return allWeights;
			}
		}

		public override float[] AllBPGradients(int test)
		{
			float[] BPGradients = new float[_nodes.Length];
			for (int node = 0; node < _nodes.Length; node++)
				BPGradients[node] = _nodes[node]._BPgradient[test];
			return BPGradients;
		}

		public LayerPerceptron(int testsCount, int nodesCount, int weightsCount, ActivationFunction af)
		{
			_af = af;
			_type = "perceptron";

			_nodes = new Node[nodesCount];
			for (int i = 0; i < _nodes.Count(); i++)
				_nodes[i] = new Node(testsCount, weightsCount);

			InitValues(testsCount);
		}

		public override void InitValues(int testsCount)
		{
			_values = new float[testsCount][][];
			for (int test = 0; test < testsCount; test++)
			{
				_values[test] = new float[1][];
				_values[test][0] = new float[_nodes.Count()];
			}

			for (int n = 0; n < _nodes.Count(); n++)
				_nodes[n].InitValues(testsCount);
		}
	}
}
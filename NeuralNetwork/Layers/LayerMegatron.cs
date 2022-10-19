using Newtonsoft.Json;
using Library;

namespace AbsurdMoneySimulations
{
	public class LayerMegatron : Layer
	{
		[JsonIgnore]
		public float[][][] _unnormalizedValues;

		public Node[] _subs; //[sub]
		public int _d;
		public int _outsPerSubCount;
		public int _lastMutatedSub;

		public override void FillWeightsRandomly()
		{
			//FillByLogic();
			//return;

			//or
			for (int sub = 0; sub < _subs.Count(); sub++)
				_subs[sub].FillRandomly();
		}

		public void FillByLogic()
		{
			float[][] ars = new float[15][];

			ars[0] = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			ars[1] = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
			ars[2] = new float[] { 1, 1, 1, -1, -1, -1, 1, 1, 1, -1, -1, -1, 1, 1, 1, -1, -1, -1, 1, 1, 1, -1, -1, -1, 1, 1, 1, -1, -1, -1 };
			ars[3] = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0.1f, 0.2f, 0.5f, 0.65f, 0.8f, 0.9f, 1, 1, 0.9f, 0.8f, 0.65f, 0.5f, 0.2f, 0.1f, 0, 0, 0, 0, 0, 0, 0, 0 };
			ars[4] = new float[] { -1.5f, -1.4f, -1.3f, -1.2f, -1.1f, -1f, -0.9f, -0.8f, -0.7f, -0.6f, -0.5f, -0.4f, -0.3f, -0.2f, -0.1f, 0, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f, 1.1f, 1.2f, 1.3f, 1.4f };

			ars[5] = new float[] { -1, -1, -1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, -1, -1, -1, -1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1 };
			ars[6] = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 };
			ars[7] = new float[] { 0, 0.5f, 1, 1, 1, 1, 0.5f, -0.5f, -1, -1, -1, -1, -0.5f, 0, 0, 0, 0, -0.5f, -1, -1, -1, -1, -0.5f, 0.5f, 1, 1, 1, 1, 0.5f, 0 };
			ars[8] = new float[] { 0, 0.5f, -1, -1, -1, -1, -0.5f, 0.5f, 1, 1, 1, 1, 0.5f, 0, 0, 0, 0, 0.5f, -1, -1, -1, -1, -0.5f, 0.5f, 1, 1, 1, 1, 0.5f, 0 };
			ars[9] = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

			ars[10] = new float[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
			ars[11] = new float[] { 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1 };
			ars[12] = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
			ars[13] = new float[] { 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1 };
			ars[14] = new float[] { 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1 };

			for (int s = 0; s < _subs.Count(); s++)
			{
				for (int w = 0; w < 30; w++)
					_subs[s]._weights[w] = ars[s][w];
			}
		}

		public override void Calculate(int test, float[][] input)
		{
			for (int sub = 0; sub < _subs.Length; sub++)
				CalculateOneSub(test, input, sub);
		}

		public override void Calculate(int test, float[] input)
		{
			throw new NotImplementedException();
		}

		public override LayerRecalculateStatus Recalculate(int test, float[][] input, LayerRecalculateStatus lrs)
		{
			if (lrs == LayerRecalculateStatus.OneWeightChanged)
			{
				CalculateOneSub(test, input, _lastMutatedSub);
				lrs = LayerRecalculateStatus.OneSubChanged;
				lrs._lastMutatedSub = _lastMutatedSub;
				return lrs;
			}
			else
			{
				Calculate(test, input);
				return LayerRecalculateStatus.Full;
			}
		}

		public override void FindBPGradient(int test, float[] innerBPGradients, float[][] innerWeights)
		{
			int gradientsPerSubCount = innerBPGradients.Count() / _subs.Count();
			for (int sub = 0; sub < _subs.Count(); sub++)
				FindBPGradientOneSub(test, sub, Array2.SubArray(innerBPGradients, sub * gradientsPerSubCount, gradientsPerSubCount), Array2.SubArray(innerWeights, sub * _outsPerSubCount, _outsPerSubCount));
		}

		public override void FindBPGradient(int test, float desiredValue)
		{
			throw new NotImplementedException();
		}

		public override void UseInertionForGradient(int test)
		{
			for (int sub = 0; sub < _subs.Count(); sub++)
				_subs[sub].UseInertionForGradient(test);
		}

		private void FindBPGradientOneSub(int test, int sub, float[] innerBPGradients, float[][] innerWeights)
		{
			float buffer = 0;
			for (int n = 0; n < _outsPerSubCount; n++)
			{
				float gwsumm = Node.FindSummOfBPGradientsPerWeights(innerBPGradients, innerWeights[n]);
				buffer += gwsumm * _af.df(_unnormalizedValues[test][sub][n]);
			}
			//buffer /= valuesPerSubCount; //!!!!!!
			_subs[sub]._BPgradient[test] += buffer;
			_subs[sub]._BPgradient[test] = _ownerNN.CutGradient(_subs[sub]._BPgradient[test]);
		}

		private void CalculateOneSub(int test, float[][] input, int sub)
		{
			for (int v = 0; v < _values[test][sub].Length; v++)
			{
				_unnormalizedValues[test][sub][v] = _subs[sub].Calculate(test, input[0], v * _d);
				_values[test][sub][v] = _af.f(_unnormalizedValues[test][sub][v]);
			}
		}

		public override void Mutate(float mutagen)
		{
			_lastMutatedSub = Storage.rnd.Next(_subs.Count());
			_subs[_lastMutatedSub].Mutate(mutagen);
		}

		public override void Demutate(float mutagen)
		{
			_subs[_lastMutatedSub].Demutate(mutagen);
		}

		public override void CorrectWeightsByBP(int test, float[][] input)
		{
			for (int sub = 0; sub < _subs.Length; sub++)
				CorrectWeightsByBPOneSub(test, sub, input[0]);
		}

		private void CorrectWeightsByBPOneSub(int test, int sub, float[] input)
		{
			for (int v = 0; v < _values[test][sub].Length; v++)
				_subs[sub].CorrectWeightsByBP(test, input, v * _d);
		}

		public override float GetAnswer(int test)
		{
			throw new NotImplementedException();
		}

		public override float[][] GetValues(int test)
		{
			return _values[test];
		}

		public override int WeightsCount
		{
			get
			{
				return _subs.Count() * _subs[0]._weights.Count();
			}
		}

		public override float[][] AllWeights
		{
			get
			{
				throw new NotImplementedException();
				//maybe somewhen it will be implemented...
				//but probably no.
			}
		}

		public override float[] AllBPGradients(int test)
		{
			throw new NotImplementedException();
		}

		public LayerMegatron(NN ownerNN, int testsCount, int subsCount, int outsPerSubCount, int weightsPerSubCount, int d, ActivationFunction af)
		{
			_ownerNN = ownerNN;
			_af = af;
			_type = "megatron";
			_d = d;
			_outsPerSubCount = outsPerSubCount;

			_subs = new Node[subsCount];
			for (int sub = 0; sub < _subs.Count(); sub++)
				_subs[sub] = new Node(ownerNN, testsCount, weightsPerSubCount);

			InitValues(testsCount);
		}

		public override void InitValues(int testsCount)
		{
			_values = new float[testsCount][][];
			for (int test = 0; test < testsCount; test++)
			{
				_values[test] = new float[_subs.Count()][];
				for (int sub = 0; sub < _subs.Count(); sub++)
					_values[test][sub] = new float[_outsPerSubCount];
			}

			_unnormalizedValues = new float[testsCount][][];
			for (int test = 0; test < testsCount; test++)
			{
				_unnormalizedValues[test] = new float[_subs.Count()][];
				for (int sub = 0; sub < _subs.Count(); sub++)
					_unnormalizedValues[test][sub] = new float[_outsPerSubCount];
			}

			for (int s = 0; s < _subs.Count(); s++)
				_subs[s].InitValues(testsCount); //does we need you? //
		}

		public override void InitLinksToOwnerNN(NN ownerNN)
		{
			_ownerNN = ownerNN;

			for (int sub = 0; sub < _subs.Count(); sub++)
				_subs[sub].SetOwnerNN(ownerNN);
		}
	}
}
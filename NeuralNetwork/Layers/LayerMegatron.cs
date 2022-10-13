using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AbsurdMoneySimulations
{
	public class LayerMegatron : LayerAbstract
	{
		[JsonIgnore]
		public float[][][] unnormalizedValues;

		public Node[] subs; //[sub]
		public int d;
		public int valuesPerSubCount; //should be
		public int lastMutatedSub;

		public override void FillWeightsRandomly()
		{
			FillByLogic();
			return;
			//////////////////////////////
			//or
			for (int sub = 0; sub < subs.Count(); sub++)
				subs[sub].FillRandomly();
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

			for (int s = 0; s < subs.Count(); s++)
			{
				for (int w = 0; w < 30; w++)
					subs[s].weights[w] = ars[s][w];
			}
		}

		public override void Calculate(int test, Tester tester, float[][] input)
		{
			for (int sub = 0; sub < subs.Length; sub++)
				CalculateOneSub(test, input, sub);
		}

		public override LayerRecalculateStatus Recalculate(int test, Tester tester, float[][] input, LayerRecalculateStatus lrs)
		{
			if (lrs == LayerRecalculateStatus.First)
			{
				CalculateOneSub(test, input, lastMutatedSub);
				lrs = LayerRecalculateStatus.OneSubChanged;
				lrs.lastMutatedSub = lastMutatedSub;
				return lrs;
			}
			else
			{
				Calculate(test, tester, input);
				return LayerRecalculateStatus.Full;
			}
		}

		public override void FindBPGradient(int test, float[] innerBPGradients, float[][] innerWeights)
		{
			int gradientsPerSubCount = innerBPGradients.Count() / subs.Count();
			for (int sub = 0; sub < subs.Count(); sub++)
				FindBPGradientOneSub(test, sub, Extensions.SubArray(innerBPGradients, sub * gradientsPerSubCount, gradientsPerSubCount), Extensions.SubArray(innerWeights, sub * valuesPerSubCount, valuesPerSubCount));
		}

		public override void FindBPGradient(int test, float desiredValue)
		{
			throw new NotImplementedException();
		}

		private void FindBPGradientOneSub(int test, int sub, float[] innerBPGradients, float[][] innerWeights)
		{
			subs[sub].BPgradient[test] = NN.INERTION * subs[sub].BPgradient[test];
			float buffer = 0;
			for (int n = 0; n < valuesPerSubCount; n++)
			{
				float gwsumm = Node.FindSummOfBPGradientsPerWeights(innerBPGradients, innerWeights[n]);
				buffer += gwsumm * af.df(unnormalizedValues[test][sub][n]);
			}
			buffer /= valuesPerSubCount; //!!!!!!
			subs[sub].BPgradient[test] += buffer;
			subs[sub].BPgradient[test] = NN.CutGradient(subs[sub].BPgradient[test]);
		}

		private void CalculateOneSub(int test, float[][] input, int sub)
		{
			for (int v = 0; v < values[test][sub].Length; v++)
			{
				unnormalizedValues[test][sub][v] = subs[sub].Calculate(test, input[0], v * d);
				values[test][sub][v] = af.f(unnormalizedValues[test][sub][v]);
			}
		}

		public override void Mutate(float mutagen)
		{
			lastMutatedSub = Storage.rnd.Next(subs.Count());
			subs[lastMutatedSub].Mutate(mutagen);
		}

		public override void Demutate(float mutagen)
		{
			subs[lastMutatedSub].Demutate(mutagen);
		}

		public override void CorrectWeightsByBP(int test, float[][] input)
		{
			for (int sub = 0; sub < subs.Length; sub++)
				CorrectWeightsByBPOneSub(test, sub, input[0]);
		}

		private void CorrectWeightsByBPOneSub(int test, int sub, float[] input)
		{
			for (int v = 0; v < values[test][sub].Length; v++)
				subs[sub].CorrectWeightsByBP(test, input, v * d);
		}

		public override float GetAnswer(int test)
		{
			throw new NotImplementedException();
		}

		public override float[][] GetValues(int test)
		{
			return values[test];
		}

		public override int WeightsCount
		{
			get
			{
				return subs.Count() * subs[0].weights.Count();
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

		public LayerMegatron(int testsCount, int subsCount, int outsPerSubCount, int weightsPerSubCount, int d)
		{
			type = 2;
			this.d = d;
			this.valuesPerSubCount = outsPerSubCount;

			subs = new Node[subsCount];
			for (int sub = 0; sub < subs.Count(); sub++)
				subs[sub] = new Node(testsCount, weightsPerSubCount);

			InitValues(testsCount);
		}

		public override void InitValues(int testsCount)
		{
			values = new float[testsCount][][];
			for (int test = 0; test < testsCount; test++)
			{
				values[test] = new float[subs.Count()][];
				for (int sub = 0; sub < subs.Count(); sub++)
					values[test][sub] = new float[valuesPerSubCount];
			}

			unnormalizedValues = new float[testsCount][][];
			for (int test = 0; test < testsCount; test++)
			{
				unnormalizedValues[test] = new float[subs.Count()][];
				for (int sub = 0; sub < subs.Count(); sub++)
					unnormalizedValues[test][sub] = new float[valuesPerSubCount];
			}

			for (int s = 0; s < subs.Count(); s++)
				subs[s].InitValues(testsCount); //does we need you? //
		}
	}
}
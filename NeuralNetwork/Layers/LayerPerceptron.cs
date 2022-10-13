using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class LayerPerceptron : LayerAbstract
	{
		public Node[] nodes;
		public int lastMutatedNode;

		public override void FillWeightsRandomly()
		{
			for (int i = 0; i < nodes.Length; i++)
				nodes[i].FillRandomly();
		}

		public override void Calculate(int test, float[][] input)
		{
			for (int node = 0; node < nodes.Length; node++)
				values[test][0][node] = af.f(nodes[node].Calculate(test, input[0], 0));
		}

		public void Calculate(int test, float[] input)
		{
			for (int node = 0; node < nodes.Length; node++)
				values[test][0][node] = af.f(nodes[node].Calculate(test, input, 0));
		}

		public override LayerRecalculateStatus Recalculate(int test, float[][] input, LayerRecalculateStatus lrs)
		{
			if (lrs == LayerRecalculateStatus.First)
			{
				values[test][0][lastMutatedNode] = af.f(nodes[lastMutatedNode].CalculateOnlyOneWeight(test, input[0][lastMutatedNode], nodes[lastMutatedNode].lastMutatedWeight));
				lrs = LayerRecalculateStatus.OneNodeChanged;
				lrs.lastMutatedNode = lastMutatedNode;
				return lrs;
			}
			else if (lrs == LayerRecalculateStatus.OneNodeChanged)
			{
				for (int n = 0; n < nodes.Length; n++)
					values[test][0][n] = af.f(nodes[n].CalculateOnlyOneWeight(test, input[0][lrs.lastMutatedNode], lrs.lastMutatedNode));
				return LayerRecalculateStatus.Full;
			}
			else if (lrs == LayerRecalculateStatus.OneSubChanged)
			{
				for (int n = 0; n < nodes.Length; n++)
				{
					for (int subnode = lrs.lastMutatedSub * lrs.subSize; subnode < lrs.lastMutatedSub * lrs.subSize + lrs.subSize; subnode++)
						nodes[n].CalculateOnlyOneWeight(test, input[0][subnode], subnode);
					//test me

					values[test][0][n] = af.f(nodes[n].summ[test]);
				}
				return LayerRecalculateStatus.Full;
			}
			else
			{
				Calculate(test, input);
				return LayerRecalculateStatus.Full;
			}
		}

		public LayerRecalculateStatus Recalculate(int test, float[] input, LayerRecalculateStatus lrs)
		{
			if (lrs == LayerRecalculateStatus.First)
			{
				values[test][0][lastMutatedNode] = af.f(nodes[lastMutatedNode].CalculateOnlyOneWeight(test, input[lastMutatedNode], nodes[lastMutatedNode].lastMutatedWeight));
				lrs = LayerRecalculateStatus.OneNodeChanged;
				lrs.lastMutatedNode = lastMutatedNode;
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
			for (int n = 0; n < nodes.Count(); n++)
				nodes[n].FindBPGradient(test, af, innerBPGradients, innerWeights[n]);
		}

		public override void FindBPGradient(int test, float desiredValue)
		{
			nodes[0].FindBPGradient(test, af, desiredValue);
		}

		public override void Mutate(float mutagen)
		{
			lastMutatedNode = Storage.rnd.Next(nodes.Count());
			nodes[lastMutatedNode].Mutate(mutagen);
		}

		public override void Demutate(float mutagen)
		{
			nodes[lastMutatedNode].Demutate(mutagen);
		}

		public override void CorrectWeightsByBP(int test, float[][] input)
		{
			for (int node = 0; node < nodes.Length; node++)
				nodes[node].CorrectWeightsByBP(test, input[0], 0);
		}

		public void CorrectWeightsByBP(int test, float[] input)
		{
			for (int node = 0; node < nodes.Length; node++)
				nodes[node].CorrectWeightsByBP(test, input, 0);
		}

		public override float[][] GetValues(int test)
		{
			return values[test];
		}

		public override float GetAnswer(int test)
		{
			return af.f(nodes[0].summ[test]);
		}

		public override int WeightsCount
		{
			get
			{
				return nodes.Count() * nodes[0].weights.Count();
			}
		}

		public override float[][] AllWeights
		{
			get
			{
				float[][] allWeights = new float[nodes[0].weights.Length][];
				for (int weight = 0; weight < nodes[0].weights.Length; weight++)
				{
					allWeights[weight] = new float[nodes.Length];
					for (int node = 0; node < nodes.Length; node++)
						allWeights[weight][node] = nodes[node].weights[weight];
				}
				return allWeights;
			}
		}

		public override float[] AllBPGradients(int test)
		{
			float[] BPGradients = new float[nodes.Length];
			for (int node = 0; node < nodes.Length; node++)
				BPGradients[node] = nodes[node].BPgradient[test];
			return BPGradients;
		}

		public LayerPerceptron(int testsCount, int nodesCount, int weightsCount)
		{
			type = 1;

			nodes = new Node[nodesCount];
			for (int i = 0; i < nodes.Count(); i++)
				nodes[i] = new Node(testsCount, weightsCount);

			InitValues(testsCount);
		}

		public override void InitValues(int testsCount)
		{
			values = new float[testsCount][][];
			for (int test = 0; test < testsCount; test++)
			{
				values[test] = new float[1][];
				values[test][0] = new float[nodes.Count()];
			}

			for (int n = 0; n < nodes.Count(); n++)
				nodes[n].InitValues(testsCount);
		}
	}
}
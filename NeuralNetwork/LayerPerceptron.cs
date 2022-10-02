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
				values[test][0][node] = nodes[node].CalculateNormalized(test, input[0], 0);
		}

		public void Calculate(int test, float[] input)
		{
			for (int node = 0; node < nodes.Length; node++)
				values[test][0][node] = nodes[node].CalculateNormalized(test, input, 0);
		}

		public override LayerRecalculateStatus Recalculate(int test, float[][] input, LayerRecalculateStatus lrs)
		{
			if (lrs == LayerRecalculateStatus.First)
			{
				values[test][0][lastMutatedNode] = nodes[lastMutatedNode].CalculateOnlyOneWeightNormalized(NNTester.testsCount, input[0][lastMutatedNode], nodes[lastMutatedNode].lastMutatedWeight);
				lrs = LayerRecalculateStatus.OneNodeChanged;
				lrs.lastMutatedNode = lastMutatedNode;
				return lrs;
			}
			else if (lrs == LayerRecalculateStatus.OneNodeChanged)
			{
				for (int n = 0; n < nodes.Length; n++)
					values[test][0][n] = nodes[n].CalculateOnlyOneWeightNormalized(NNTester.testsCount, input[0][lrs.lastMutatedNode], lrs.lastMutatedNode);
				return LayerRecalculateStatus.Full;
			}
			else if (lrs == LayerRecalculateStatus.OneSubChanged)
			{
				for (int n = 0; n < nodes.Length; n++)
				{
					for (int subnode = lrs.lastMutatedSub * lrs.subSize; subnode < lrs.lastMutatedSub * lrs.subSize + lrs.subSize; subnode++)
						nodes[n].CalculateOnlyOneWeightNormalized(NNTester.testsCount, input[0][subnode], subnode);
					
					values[test][0][n] = ActivationFunctions.Normalize(nodes[n].summ[test]);
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
				values[test][0][lastMutatedNode] = nodes[lastMutatedNode].CalculateOnlyOneWeightNormalized(test, input[lastMutatedNode], nodes[lastMutatedNode].lastMutatedWeight);
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
				nodes[n].FindBPGradient(test, innerBPGradients, innerWeights[n]);
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

		public override float[][] GetValues(int test)
		{
			return values[test];
		}

		public override float GetAnswer(int test)
		{
			return nodes[0].summ[test];
		}

		public override int WeightsCount
		{
			get
			{
				return nodes.Count() * nodes[0].weights.Count();
			}
		}

		public LayerPerceptron(int nodesCount, int weightsCount)
		{
			type = 1;

			nodes = new Node[nodesCount];
			for (int i = 0; i < nodes.Count(); i++)
				nodes[i] = new Node(weightsCount);

			InitValues();
		}

		public override void InitValues()
		{
			values = new float[NNTester.testsCount][][];
			for (int test = 0; test < NNTester.testsCount; test++)
			{
				values[test] = new float[1][];
				values[test][0] = new float[nodes.Count()];
			}

			for (int n = 0; n < nodes.Count(); n++)
				nodes[n].InitValues();
		}
	}
}

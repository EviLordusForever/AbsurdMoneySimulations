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

		public override void FillRandomly(int subsCount, int nodesCount, int weightsCount)
		{
			nodes = new Node[nodesCount];
			for (int i = 0; i < nodesCount; i++)
			{
				nodes[i] = new Node(weightsCount);
				nodes[i].FillRandomly();
			}
		}

		public override void Calculate(int test, float[][] input)
		{
			for (int node = 0; node < nodes.Length; node++)
				values[test][0][node] = nodes[node].Calculate(input[0], 0);
		}

		public void Calculate(int test, float[] input)
		{
			for (int node = 0; node < nodes.Length; node++)
				values[test][0][node] = nodes[node].Calculate(input, 0);
		}

		public override LayerRecalculateStatus Recalculate(int test, float[][] input, LayerRecalculateStatus lrs)
		{
			if (lrs == LayerRecalculateStatus.First)
			{
				values[test][0][lastMutatedNode] = nodes[lastMutatedNode].CalculateOnlyOneWeight(input[0][lastMutatedNode], nodes[lastMutatedNode].lastMutatedWeight);
				lrs = LayerRecalculateStatus.OneNodeChanged;
				lrs.lastMutatedNode = lastMutatedNode;
				return lrs;
			}
			else if (lrs == LayerRecalculateStatus.OneNodeChanged)
			{
				for (int n = 0; n < nodes.Length; n++)
					values[test][0][n] = nodes[n].CalculateOnlyOneWeight(input[0][lrs.lastMutatedNode], lrs.lastMutatedNode);
				return LayerRecalculateStatus.Full;
			}
			else if (lrs == LayerRecalculateStatus.OneSubChanged)
			{
				for (int n = 0; n < nodes.Length; n++)
				{
					for (int subnode = lrs.lastMutatedSub * lrs.subSize; subnode < lrs.lastMutatedSub * lrs.subSize + lrs.subSize; subnode++)
						nodes[n].CalculateOnlyOneWeight(input[0][subnode], subnode);
					
					values[test][0][n] = Brain.Normalize(nodes[n].summ);
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
				values[test][0][lastMutatedNode] = nodes[lastMutatedNode].CalculateOnlyOneWeight(input[lastMutatedNode], nodes[lastMutatedNode].lastMutatedWeight);
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

		public override int WeightsCount
		{
			get
			{
				return nodes.Count() * nodes[0].weights.Count();
			}
		}

		public LayerPerceptron(int nodesCount)
		{
			values = new float[NNTester.testsCount][][];
			for (int test = 0; test < NNTester.testsCount; test++)
			{
				values[test] = new float[1][];
				values[test][0] = new float[nodesCount];
			}

			type = 1;
		}
	}
}

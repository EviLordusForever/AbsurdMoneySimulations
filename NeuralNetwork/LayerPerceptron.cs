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
				nodes[i] = new Node();
				nodes[i].FillRandomly(weightsCount);
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

		public override void Mutate(float mutagen)
		{
			lastMutatedNode = Storage.rnd.Next(nodes.Count());
			nodes[lastMutatedNode].Mutate(mutagen);
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

		public void CalculateOneNode(int test)
		{
			values[test][0][lastMutatedNode] = nodes[lastMutatedNode].Calculate(NNTester.tests[test], 0);
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

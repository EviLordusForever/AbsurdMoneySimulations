using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class LayerPerceptron : AbstractLayer
	{
		public Node[] nodes;
		//public double[][][] values;

		public override void FillRandomly(int subsCount, int nodesCount, int weightsCount)
		{
			nodes = new Node[nodesCount];
			for (int i = 0; i < nodesCount; i++)
			{
				nodes[i] = new Node();
				nodes[i].FillRandomly(weightsCount);
			}
		}

		public override void Calculate(int test, double[] input)
		{
			for (int node = 0; node < nodes.Length; node++)
				values[test][0][node] = nodes[node].Calculate(input, 0);
		}

		public void CalculateOneNode(int test, double[] input, int start, int node)
		{
			values[test][0][node] = nodes[node].Calculate(input, start);
		}

		public LayerPerceptron()
		{
		}
	}
}

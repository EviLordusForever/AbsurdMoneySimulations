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
		public double[][] values;

		public override void FillRandomly(int subsCount, int nodesCount, int weightsCount)
		{
			nodes = new Node[nodesCount];
			for (int i = 0; i < nodesCount; i++)
			{
				nodes[i] = new Node();
				nodes[i].FillRandomly(weightsCount);
			}
		}

		public void Calculate(int test, double[] input, int start)
		{
			for (int node = 0; node < nodes.Length; node++)
				values[test][node] = nodes[node].Calculate(input, start);
		}

		public void CalculateOneNode(int test, double[] input, int start, int node)
		{
			values[test][node] = nodes[node].Calculate(input, start);
		}

		public LayerPerceptron(string[] nodesStrs)
		{
			nodes = new Node[nodesStrs.Length];

			for (int n = 0; n < nodesStrs.Count(); n++)
			{
				string[] weightsStrs = nodesStrs[n].Split('w');
				nodes[n] = new Node(weightsStrs);
			}
		}

		public LayerPerceptron()
		{
		}
	}
}

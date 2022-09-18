using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class LayerMegatron : AbstractLayer
	{
		public Node[] subs; //[sub]
		public double[][][] values; //[test][sub][node]
		public int d;

		public override void FillRandomly(int subsCount, int nodesCount, int weightsCount)
		{
			subs = new Node[subsCount];
			for (int sub = 0; sub < subsCount; sub++)
			{
				subs[sub] = new Node();
				subs[sub].FillRandomly(weightsCount);
			}
		}

		public override void Calculate(int test, double[] input)
		{
			for (int sub = 0; sub < subs.Length; sub++)
				for (int node = 0; node < values[test][sub].Length; node++)
					values[test][sub][node] = subs[sub].Calculate(input, node * d);
		}

		public void CalculateOneSub(int test, double[] input, int sub)
		{
			for (int node = 0; node < values[test][sub].Length; node++)
				values[test][sub][node] = subs[sub].Calculate(input, node * d); //(!)
		}

		public void CalculateOneNode(int test, double[] input, int sub, int node)
		{
			values[test][sub][node] = subs[sub].Calculate(input, node * d);
		}

		public LayerMegatron()
		{
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class Node
	{
		public double[] weights;

		public void FillRandomly(int weightsCount)
		{
			weights = new double[weightsCount];
			for (int i = 0; i < weightsCount; i++)
				weights[i] = (Storage.rnd.NextDouble() * 2 - 1) * NN.randomPower; /////////////////
		}

		public double Calculate(double[] input, int start)
		{
			double res = 0;

			for (int i = 0; i < weights.Count(); i++)
				res += weights[i] * input[start + i];

			return Brain.Normalize(res);
		}

		public Node(string[] weightsStrs)
		{
			weights = new double[weightsStrs.Count()];

			for (int w = 0; w < weightsStrs.Count(); w++)
				weights[w] = Convert.ToDouble(weightsStrs[w]);
		}

		public Node()
		{
		}

		public override string ToString()
		{
			return String.Join("w", weights);
		}
	}
}


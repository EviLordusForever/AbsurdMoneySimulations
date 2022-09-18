using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public abstract class AbstractLayer
	{
		public double[][][] values;

		public abstract void FillRandomly(int subsCount, int nodesCount, int weightsCount);

		public abstract void Calculate(int test, double[] input);
	}
}

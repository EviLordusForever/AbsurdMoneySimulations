using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations.NeuralNetwork.ActivationFunctions
{
	public class SoftSign : ActivationFunction
	{
		public override float f(float x)
		{
			return MathF.Abs(x) / (1 + MathF.Abs(x));
		}

		public override float df(float x)
		{
			return 1 / (2 * MathF.Abs(x) + x * x + 1);
		}
	}
}

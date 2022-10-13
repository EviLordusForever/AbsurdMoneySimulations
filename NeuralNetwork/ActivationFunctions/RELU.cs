using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class ReLU : ActivationFunction
	{
		public override float f(float x)
		{
			if (x > 0)
				return x;
			else
				return 0;
		}

		public override float df(float x)
		{
			if (x > 0)
				return 1;
			else
				return 0;
		}
	}
}

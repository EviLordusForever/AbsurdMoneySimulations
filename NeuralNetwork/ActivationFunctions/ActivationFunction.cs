using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public abstract class ActivationFunction
	{
		public abstract float f(float x);

		public abstract float df(float x);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class ClassicAF : ActivationFunction
	{
		public override float f(float x)
		{
			return 2f / (1 + MathF.Pow(1.1f, -x)) - 1;
		}

		public override float df(float x)
		{
			return 0.19062f * MathF.Pow(1.1f, -x) / MathF.Pow(MathF.Pow(1.1f, -x) + 1, 2);
		}
	}
}

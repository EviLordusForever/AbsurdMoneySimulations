using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class ActivationFunctions
	{
		public static float Normalize(float x)
		{
			return 2f / (1 + MathF.Pow(1.1f, -x)) - 1;
		}

		public static float DerivativeOfNormilize(float x)
		{
			return 0.19062f * MathF.Pow(1.1f, -x) / MathF.Pow(MathF.Pow(1.1f, -x) + 1, 2);
		}
	}
}

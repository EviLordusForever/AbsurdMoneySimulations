using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class Brain
	{
        public static double Interpolate(double from, double to, double actual)
        {
            return (actual - from) / (to - from);
        }

        public static double Normalize(double input)
        {
            return (double)(2 * 1 / (1 + Math.Pow(1.1, -input)) - 1);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class Linear : ActivationFunction
	{
		public override float f(float x)
		{
			return x;
		}

		public override float df(float x)
		{
			return 1;
		}
	}
}

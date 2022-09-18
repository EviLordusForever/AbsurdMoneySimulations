using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public abstract class AbstractLayer
	{
		public abstract void FillRandomly(int subsCount, int nodesCount, int weightsCount);
	}
}

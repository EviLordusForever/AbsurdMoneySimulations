using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class LayerRecalculateStatus
	{
		//This is hard to explain
		//Layers are go after layers
		//If NN mutated you have no reason
		//to recalculate everything again
		//You need to recalculate only part of it
		//Beginning from the node on First layer
		//Than part of Second Layer
		//And so on...
		//The problem is that
		//this is different for every type of layers

		private string status;
		public int lastMutatedNode;
		public int lastMutatedSub;
		public int subSize;

		public string Status 
		{
			get
			{
				return status;
			}
		}

		public static LayerRecalculateStatus First
		{
			get
			{
				return new LayerRecalculateStatus { status = "First" };
			}
		}

		public static LayerRecalculateStatus OneNodeChanged
		{
			get
			{
				return new LayerRecalculateStatus { status = "OneNodeChanged" };
			}
		}

		public static LayerRecalculateStatus OneSubChanged
		{
			get
			{
				return new LayerRecalculateStatus { status = "OneSubChanged" };
			}
		}

		public static LayerRecalculateStatus Full
		{
			get
			{
				return new LayerRecalculateStatus { status = "Full" };
			}
		}
	}
}

namespace AbsurdMoneySimulations
{
	public class LayerRecalculateStatus
	{
		//It is hard to explain
		//Layers are go after layers
		//If NN mutated you have no reason
		//to recalculate everything again
		//You need to recalculate only part of it
		//Beginning from the node on First layer
		//Than part of Second Layer
		//And so on...
		//The problem is that
		//this is different for every type of layers

		private string _status;
		public int _lastMutatedNode;
		public int _lastMutatedSub;
		public int _subSize;

		public string Status
		{
			get
			{
				return _status;
			}
		}

		public static LayerRecalculateStatus OneWeightChanged
		{
			get
			{
				return new LayerRecalculateStatus { _status = "First" };
			}
		}

		public static LayerRecalculateStatus OneNodeChanged
		{
			get
			{
				return new LayerRecalculateStatus { _status = "OneNodeChanged" };
			}
		}

		public static LayerRecalculateStatus OneSubChanged
		{
			get
			{
				return new LayerRecalculateStatus { _status = "OneSubChanged" };
			}
		}

		public static LayerRecalculateStatus Full
		{
			get
			{
				return new LayerRecalculateStatus { _status = "Full" };
			}
		}
	}
}

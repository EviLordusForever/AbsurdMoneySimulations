﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class LayerInput : LayerAbstract
	{
		public Node[] nodes;

		public override void FillRandomly(int subsCount, int nodesCount, int weightsCount)
		{
			return;
		}

		public override void Calculate(int test, double[] input)
		{
			return;
		}

		public override void Mutate(double mutagen)
		{
			return;
		}

		public override int WeightsCount
		{
			get
			{
				return 0;
			}
		}

		public LayerInput(int size)
		{
			nodes = new Node[size];
		}
	}
}


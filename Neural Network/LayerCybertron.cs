using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class LayerCybertron : LayerAbstract
	{
		public LayerPerceptron[] subs;
		public int lastMutatedSub;

		public override void FillRandomly(int subsCount, int nodesCount, int weightsCount)
		{
			subs = new LayerPerceptron[subsCount];
			for (int i = 0; i < subsCount; i++)
			{
				subs[i] = new LayerPerceptron();
				subs[i].FillRandomly(subsCount, nodesCount, weightsCount);
			}
		}

		public override void Calculate(int test, double[] input)
		{
			int len = input.Length / subs.Length;

			for (int sub = 0; sub < subs.Length; sub++)
				subs[sub].Calculate(test, Brain.SubArray(input, len * sub, len));
			//Test me
		}

		public override void Mutate(double mutagen)
		{
			lastMutatedSub = Storage.rnd.Next(subs.Count());
			subs[lastMutatedSub].Mutate(mutagen);
		}

		public override int WeightsCount
		{
			get
			{
				return subs.Count() * subs[0].WeightsCount;
			}
		}

		public LayerCybertron()
		{
		}
	}
}

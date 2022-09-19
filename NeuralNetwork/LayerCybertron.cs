using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class LayerCybertron : LayerAbstract
	{
		public LayerPerceptron[] perceptrons;
		public int lastMutatedSub;

		public override void FillRandomly(int subsCount, int nodesCount, int weightsCount)
		{
			perceptrons = new LayerPerceptron[subsCount];
			for (int i = 0; i < subsCount; i++)
			{
				perceptrons[i] = new LayerPerceptron();
				perceptrons[i].FillRandomly(subsCount, nodesCount, weightsCount);
			}
		}

		public override void Calculate(int test, float[][] input)
		{
			for (int sub = 0; sub < perceptrons.Length; sub++)
				perceptrons[sub].Calculate(test, input[sub]);
			//Test me
		}

		public override void Mutate(float mutagen)
		{
			lastMutatedSub = Storage.rnd.Next(perceptrons.Count());
			perceptrons[lastMutatedSub].Mutate(mutagen);
		}

		public override int WeightsCount
		{
			get
			{
				return perceptrons.Count() * perceptrons[0].WeightsCount;
			}
		}

		public LayerCybertron()
		{
			type = 3;
		}
	}
}

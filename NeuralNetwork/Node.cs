using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class Node
	{
		public float[] weights;
		public float[] subvalues;
		public float summ;
		public int lastMutatedWeight;

		public void FillRandomly()
		{
			for (int i = 0; i < weights.Count(); i++)
				weights[i] = (Storage.rnd.NextSingle() * 2 - 1) * NN.randomPower; /////////////////
		}

		public float Calculate(float[] input, int start)
		{
			summ = 0;

			for (int i = 0; i < weights.Count(); i++)
			{
				subvalues[i] = weights[i] * input[start + i];
				summ += subvalues[i];
			}

			return Brain.Normalize(summ);
		}

		public float CalculateOnlyOneWeight(float input, int w)
		{
			summ -= subvalues[w];
			subvalues[w] = weights[w] * input;
			summ += subvalues[w];

			return Brain.Normalize(summ);
		}

		public void Mutate(float mutagen)
		{
			lastMutatedWeight = Storage.rnd.Next(weights.Length);
			weights[lastMutatedWeight] += mutagen;
		}

		public void Demutate(float mutagen)
		{
			weights[lastMutatedWeight] -= mutagen;
		}

		public Node(int weightsCount)
		{
			weights = new float[weightsCount];
			subvalues = new float[weightsCount];
		}
	}
}


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class Node
	{
		public float[] weights;
		public float[][] subvalues;
		public float[] summ;
		public int lastMutatedWeight;

		public void FillRandomly()
		{
			for (int i = 0; i < weights.Count(); i++)
				weights[i] = (Storage.rnd.NextSingle() * 2 - 1) * NN.randomPower; /////////////////
		}

		public float Calculate(int test, float[] input, int start)
		{
			summ[test] = 0;

			for (int i = 0; i < weights.Count(); i++)
			{
				subvalues[test][i] = weights[i] * input[start + i];
				summ[test] += subvalues[test][i];
			}

			return Brain.Normalize(summ[test]);
		}

		public float CalculateOnlyOneWeight(int test, float input, int w)
		{
			summ[test] -= subvalues[test][w];
			subvalues[test][w] = weights[w] * input;
			summ[test] += subvalues[test][w];

			return Brain.Normalize(summ[test]);
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

		public Node(int testsCount, int weightsCount)
		{
			weights = new float[weightsCount];

			summ = new float[testsCount];

			subvalues = new float[testsCount][];

			for (int test = 0; test < testsCount; test++)
				subvalues[test] = new float[weightsCount];
		}
	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static AbsurdMoneySimulations.ActivationFunctions;

namespace AbsurdMoneySimulations
{
	public class Node
	{
		public float[] weights;

		[JsonIgnore]
		public float[][] subvalues;

		[JsonIgnore]
		public float[] summ;

		[JsonIgnore]
		public float[] BPgradient;

		public int lastMutatedWeight;

		public void FillRandomly()
		{
			for (int i = 0; i < weights.Count(); i++)
				weights[i] = (Storage.rnd.NextSingle() * 2 - 1) * NN.randomPower; /////////////////
		}

		public float CalculateNormalized(int test, float[] input, int start)
		{
			summ[test] = 0;

			for (int w = 0; w < weights.Count(); w++)
			{
				subvalues[test][w] = weights[w] * input[start + w];
				summ[test] += subvalues[test][w];
			}

			return ActivationFunction(summ[test]);
		}

		public float CalculateOnlyOneWeightNormalized(int test, float input, int w)
		{
			summ[test] -= subvalues[test][w];
			subvalues[test][w] = weights[w] * input;
			summ[test] += subvalues[test][w];

			return ActivationFunction(summ[test]);
		}

		public float CalculateNotNormalized(int test, float[] input, int start)
		{
			summ[test] = 0;

			for (int w = 0; w < weights.Count(); w++)
			{
				subvalues[test][w] = weights[w] * input[start + w];
				summ[test] += subvalues[test][w];
			}

			return summ[test];
		}

		public float CalculateOnlyOneWeightNotNormalized(int test, float input, int w)
		{
			summ[test] -= subvalues[test][w];
			subvalues[test][w] = weights[w] * input;
			summ[test] += subvalues[test][w];

			return summ[test];
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

		public void FindBPGradient(int test, float desiredValue)
		{
			BPgradient[test] = (ActivationFunction(summ[test]) - desiredValue) * DerivativeOfActivationFunction(summ[test]);
		}

		public void FindBPGradient(int test, float[] gradients, float[] weights)
		{
			float gwsumm = FindSummOfBPGradientsPerWeights(gradients, weights);
			BPgradient[test] = NN.INERTION * BPgradient[test] + gwsumm * DerivativeOfActivationFunction(summ[test]);
		}

		public static float FindSummOfBPGradientsPerWeights(float[] gradients, float[] weights)
		{
			float gwsumm = 0;

			for (int gw = 0; gw < gradients.Count(); gw++)
				gwsumm += gradients[gw] * weights[gw];

			return gwsumm;
		}

		public void CorrectWeightsByBP(int test, float[] input, int start)
		{
			for (int w = 0; w < weights.Count(); w++)
				weights[w] -= NN.LYAMBDA * BPgradient[test] * input[start + w];
		}

		public Node(int weightsCount)
		{
			weights = new float[weightsCount];

			InitValues();
		}

		public void InitValues()
		{
			summ = new float[NNTester.testsCount];

			subvalues = new float[NNTester.testsCount][];

			for (int test = 0; test < NNTester.testsCount; test++)
				subvalues[test] = new float[weights.Count()];

			BPgradient = new float[NNTester.testsCount];
		}
	}
}


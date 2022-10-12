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
		public int outNodesSummCount;

		public override void FillWeightsRandomly()
		{
			for (int i = 0; i < perceptrons.Count(); i++)
				perceptrons[i].FillWeightsRandomly();
		}

		public override void Calculate(int test, Tester tester, float[][] input)
		{
			for (int sub = 0; sub < perceptrons.Length; sub++)
				perceptrons[sub].Calculate(test, tester, input[sub]);
			//Test me
		}

		public override LayerRecalculateStatus Recalculate(int test, Tester tester, float[][] input, LayerRecalculateStatus lrs)
		{
			if (lrs == LayerRecalculateStatus.First)
			{
				perceptrons[lastMutatedSub].Recalculate(test, tester, input[lastMutatedSub], LayerRecalculateStatus.First);
				lrs.lastMutatedSub = lastMutatedSub;
				return LayerRecalculateStatus.OneSubChanged;
			}
			else if (lrs == LayerRecalculateStatus.OneSubChanged)
			{
				perceptrons[lastMutatedSub].Recalculate(test, tester, input[lastMutatedSub], LayerRecalculateStatus.Full);
				lrs.lastMutatedSub = lastMutatedSub;
				return LayerRecalculateStatus.OneSubChanged;
			}
			else
			{
				Calculate(test, tester, input);
				return LayerRecalculateStatus.Full;
			}
		}

		public override void FindBPGradient(int test, float[] innerBPGradients, float[][] innerWeights)
		{
			//very hard to explain without drawing
			for (int n = 0; n < perceptrons.Count(); n++)
				perceptrons[n].FindBPGradient(test, innerBPGradients, Extensions.SubArray(innerWeights, n * perceptrons[0].nodes.Count(), perceptrons[0].nodes.Count()));
		}

		public override void FindBPGradient(int test, float desiredValue)
		{
			throw new NotImplementedException();
		}

		public override void Mutate(float mutagen)
		{
			lastMutatedSub = Storage.rnd.Next(perceptrons.Count());
			perceptrons[lastMutatedSub].Mutate(mutagen);
		}

		public override void Demutate(float mutagen)
		{
			perceptrons[lastMutatedSub].Demutate(mutagen);
		}

		public override void CorrectWeightsByBP(int test, float[][] input)
		{
			for (int sub = 0; sub < perceptrons.Length; sub++)
				perceptrons[sub].CorrectWeightsByBP(test, input[sub]);
		}

		public override float GetAnswer(int test)
		{
			throw new NotImplementedException();
		}

		public override float[][] GetValues(int test)
		{
			int node1 = 0;
			for (int perceptron = 0; perceptron < perceptrons.Count(); perceptron++)
			{
				for (int node2 = 0; node2 < perceptrons[perceptron].nodes.Length; node2++)
					values[test][0][node1] = perceptrons[perceptron].values[test][0][node2];
				
				node1++;
			}

			return values[test];
		}

		public override int WeightsCount
		{
			get
			{
				return perceptrons.Count() * perceptrons[0].WeightsCount;
			}
		}

		public override float[][] AllWeights
		{
			get
			{
				float[][] weights = perceptrons[0].AllWeights;

				for (int p = 1; p < perceptrons.Length; p++)
					weights = Extensions.Concatenate(weights, perceptrons[p].AllWeights);

				return weights;
				//TEST THIS !!!!!
			}
		}

		public override float[] AllBPGradients(int test)
		{
			float[] gradients = perceptrons[0].AllBPGradients(test);

			for (int p = 1; p < perceptrons.Length; p++)
				gradients = Extensions.Concatenate(gradients, perceptrons[p].AllBPGradients(test));

			return gradients;
			//And this
		}

		public LayerCybertron(int testsCount, int perceptronsCount, int weightsPerNodePerceptronCount, int nodesPerPerceptronCount, int outNodesSummCount)
		{
			type = 3;

			this.outNodesSummCount = outNodesSummCount;

			perceptrons = new LayerPerceptron[perceptronsCount];
			for (int p = 0; p < perceptrons.Count(); p++)
				perceptrons[p] = new LayerPerceptron(testsCount, nodesPerPerceptronCount, weightsPerNodePerceptronCount);

			InitValues(testsCount);
		}

		public override void InitValues(int testsCount)
		{
			values = new float[testsCount][][];
			for (int test = 0; test < testsCount; test++)
			{
				values[test] = new float[1][];
				values[test][0] = new float[outNodesSummCount];
			}

			for (int p = 0; p < perceptrons.Count(); p++)
			{
				perceptrons[p].InitValues(testsCount);
				perceptrons[p].af = af = new ClassicAF();
			}
		}
	}
}

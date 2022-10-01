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

		public override void Calculate(int test, float[][] input)
		{
			for (int sub = 0; sub < perceptrons.Length; sub++)
				perceptrons[sub].Calculate(test, input[sub]);
			//Test me
		}

		public override LayerRecalculateStatus Recalculate(int test, float[][] input, LayerRecalculateStatus lrs)
		{
			if (lrs == LayerRecalculateStatus.First)
			{
				perceptrons[lastMutatedSub].Recalculate(test, input[lastMutatedSub], LayerRecalculateStatus.First);
				lrs.lastMutatedSub = lastMutatedSub;
				return LayerRecalculateStatus.OneSubChanged;
			}
			else if (lrs == LayerRecalculateStatus.OneSubChanged)
			{
				perceptrons[lastMutatedSub].Recalculate(test, input[lastMutatedSub], LayerRecalculateStatus.Full);
				lrs.lastMutatedSub = lastMutatedSub;
				return LayerRecalculateStatus.OneSubChanged;
			}
			else
			{
				Calculate(test, input);
				return LayerRecalculateStatus.Full;
			}
		}

		public override void GoBack(float[][] output)
		{
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

		public LayerCybertron(int perceptronsCount, int weightsPerNodePerceptronCount, int nodesPerPerceptronCount, int outNodesSummCount)
		{
			type = 3;

			this.outNodesSummCount = outNodesSummCount;

			perceptrons = new LayerPerceptron[perceptronsCount];
			for (int p = 0; p < perceptrons.Count(); p++)
				perceptrons[p] = new LayerPerceptron(nodesPerPerceptronCount, weightsPerNodePerceptronCount);

			InitValues();
		}

		public override void InitValues()
		{
			values = new float[NNTester.testsCount][][];
			for (int test = 0; test < NNTester.testsCount; test++)
			{
				values[test] = new float[1][];
				values[test][0] = new float[outNodesSummCount];
			}

			for (int p = 0; p < perceptrons.Count(); p++)
				perceptrons[p].InitValues();
		}
	}
}

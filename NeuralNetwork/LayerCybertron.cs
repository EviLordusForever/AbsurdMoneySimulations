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
				perceptrons[i] = new LayerPerceptron(nodesCount);
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

		public LayerCybertron(int valueNodesCount)
		{
			values = new float[NNTester.testsCount][][];
			for (int test = 0; test < NNTester.testsCount; test++)
			{
				values[test] = new float[1][];
				values[test][0] = new float[valueNodesCount];
			}

			type = 3;
		}
	}
}

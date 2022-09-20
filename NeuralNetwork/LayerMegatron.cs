using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public class LayerMegatron : LayerAbstract
	{
		public Node[] subs; //[sub]
		public float[][][] values; //[test][sub][node]
		public int d;
		public int lastMutatedSub;

		public override void FillRandomly(int subsCount, int nodesCount, int weightsCount)
		{
			subs = new Node[subsCount];
			for (int sub = 0; sub < subsCount; sub++)
			{
				subs[sub] = new Node();
				subs[sub].FillRandomly(weightsCount);
			}
		}

		public override void Calculate(int test, float[][] input)
		{
			for (int sub = 0; sub < subs.Length; sub++)
				CalculateOneSub(test, input, sub);
		}

		private void CalculateOneSub(int test, float[][] input, int sub)
		{
			for (int node = 0; node < values[test][sub].Length; node++)
				values[test][sub][node] = subs[sub].Calculate(input[0], node * d);
		}

		public override LayerRecalculateStatus Recalculate(int test, float[][] input, LayerRecalculateStatus lrs)
		{
			if (lrs == LayerRecalculateStatus.First)
			{
				CalculateOneSub(test, input, lastMutatedSub);
				lrs = LayerRecalculateStatus.OneSubChanged;
				lrs.lastMutatedSub = lastMutatedSub;
				return lrs;
			}
			else
			{
				Calculate(test, input);
				return LayerRecalculateStatus.Full;
			}
		}

		public override void Mutate(float mutagen)
		{
			lastMutatedSub = Storage.rnd.Next(subs.Count());
			subs[lastMutatedSub].Mutate(mutagen);
		}

		public override float[][] GetValues(int test)
		{
			return values[test];
		}

		public override int WeightsCount
		{
			get
			{
				return subs.Count() * subs[0].weights.Count();
			}
		}

		public void CalculateOneSub(int test, float[] input, int sub)
		{
			for (int node = 0; node < values[test][sub].Length; node++)
				values[test][sub][node] = subs[sub].Calculate(input, node * d); //(!)
		}

		public void CalculateOneNode(int test, float[] input, int sub, int node)
		{
			values[test][sub][node] = subs[sub].Calculate(input, node * d);
		}

		public LayerMegatron(int subsCount, int nodesCount)
		{
			values = new float[NNTester.testsCount][][];
			for (int test = 0; test < NNTester.testsCount; test++)
			{
				values[test] = new float[subsCount][];
				for (int sub = 0; sub < subsCount; sub++)
				{
					values[test][sub] = new float[nodesCount];
				}
			}

			type = 2;
		}
	}
}

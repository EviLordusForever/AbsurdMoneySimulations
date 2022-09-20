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
				subs[sub] = new Node(weightsCount);
				subs[sub].FillRandomly();
			}
		}

		public override void Calculate(int test, float[][] input)
		{
			for (int sub = 0; sub < subs.Length; sub++)
				CalculateOneSub(test, input, sub);
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

		private void CalculateOneSub(int test, float[][] input, int sub)
		{
			for (int node = 0; node < values[test][sub].Length; node++)
				values[test][sub][node] = subs[sub].Calculate(input[0], node * d);
		}
	
		public override void Mutate(float mutagen)
		{
			lastMutatedSub = Storage.rnd.Next(subs.Count());
			subs[lastMutatedSub].Mutate(mutagen);
		}

		public override void Demutate(float mutagen)
		{
			subs[lastMutatedSub].Demutate(mutagen);
		}

		public override float GetAnswer(int test)
		{
			throw new NotImplementedException();
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

		public LayerMegatron(int subsCount, int nodesCount, int d)
		{
			this.d = d;
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

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
			FillByLogic(subsCount, nodesCount, weightsCount);
			return;
			//////////////////////////////

			for (int sub = 0; sub < subsCount; sub++)
			{
				subs[sub] = new Node(NNTester.testsCount, weightsCount);
				subs[sub].FillRandomly();
			}
		}

		public void FillByLogic(int subsCount, int nodesCount, int weightsCount)
		{
			float[][] ars = new float[15][];

			ars[0] = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			ars[1] = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
			ars[2] = new float[] { 1, 1, 1, -1, -1, -1, 1, 1, 1, -1, -1, -1, 1, 1, 1, -1, -1, -1, 1, 1, 1, -1, -1, -1, 1, 1, 1, -1, -1, -1 };
			ars[3] = new float[] { 0, 0, 0, 0, 0,  0, 0, 0, 0.1f, 0.2f,  0.5f, 0.65f, 0.8f, 0.9f, 1,  1, 0.9f, 0.8f, 0.65f, 0.5f,  0.2f, 0.1f, 0, 0, 0,  0, 0, 0, 0, 0 };
			ars[4] = new float[] { -1.5f, -1.4f, -1.3f, -1.2f, -1.1f, -1f, -0.9f, -0.8f, -0.7f, -0.6f, -0.5f, -0.4f, -0.3f, -0.2f, -0.1f, 0, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f, 1.1f, 1.2f, 1.3f, 1.4f };

			ars[5] = new float[] { -1, -1, -1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, -1, -1, -1, -1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1 };
			ars[6] = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 };
			ars[7] = new float[] { 0, 0.5f, 1, 1, 1, 1, 0.5f, -0.5f, -1, -1, -1, -1, -0.5f, 0, 0, 0, 0, -0.5f, -1, -1, -1, -1, -0.5f, 0.5f, 1, 1, 1, 1, 0.5f, 0 };
			ars[8] = new float[] { 0, 0.5f, -1, -1, -1, -1, -0.5f, 0.5f, 1, 1, 1, 1, 0.5f, 0, 0, 0, 0, 0.5f, -1, -1, -1, -1, -0.5f, 0.5f, 1, 1, 1, 1, 0.5f, 0 };
			ars[9] = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

			ars[10] = new float[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
			ars[11] = new float[] { 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1 };
			ars[12] = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
			ars[13] = new float[] { 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1 };
			ars[14] = new float[] { 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1 };

			for (int s = 0; s < subs.Count(); s++)
			{
				subs[s] = new Node(NNTester.testsCount, weightsCount);
				for (int w = 0; w < 30; w++)
					subs[s].weights[w] = ars[s][w];
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
				values[test][sub][node] = subs[sub].Calculate(test, input[0], node * d);
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

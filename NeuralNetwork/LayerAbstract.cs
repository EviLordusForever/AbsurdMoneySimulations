using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public abstract class LayerAbstract
	{
		public float[][][] values;

		public int type;

		public abstract void FillRandomly(int subsCount, int nodesCount, int weightsCount);

		public abstract void Calculate(int test, float[][] input);

		public abstract LayerRecalculateStatus Recalculate(int test, float[][] input, LayerRecalculateStatus lrs);
		//You don't need to calculate whole NN if only part of it mutated

		public abstract void Mutate(float mutagen);

		public abstract void Demutate(float mutagen);

		public abstract float[][] GetValues(int test);

		public abstract int WeightsCount { get; }

	}
}

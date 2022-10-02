using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AbsurdMoneySimulations
{
	public abstract class LayerAbstract
	{
		[JsonIgnore]
		public float[][][] values;

		public int type;

		public abstract void FillWeightsRandomly();

		public abstract void Calculate(int test, float[][] input);

		public abstract LayerRecalculateStatus Recalculate(int test, float[][] input, LayerRecalculateStatus lrs);
		//You don't need to calculate whole NN if only part of it is mutated

		public abstract void FindBPGradient(int test, float[] innerBPGradients, float[][] innerWeights);

		public abstract void Mutate(float mutagen);

		public abstract void Demutate(float mutagen);

		public abstract float[][] GetValues(int test);

		public abstract float GetAnswer(int test);

		public abstract int WeightsCount { get; }

		public abstract void InitValues();

	}
}

﻿using Newtonsoft.Json;

namespace AbsurdMoneySimulations
{
	public abstract class LayerAbstract
	{
		[JsonIgnore]
		public float[][][] _values; //[test][sub][value]

		[JsonIgnore]
		public ActivationFunction _af;

		public int _type;

		public abstract void FillWeightsRandomly();

		public abstract void Calculate(int test, float[][] input);

		public abstract void Calculate(int test, float[] input);

		public abstract LayerRecalculateStatus Recalculate(int test, float[][] input, LayerRecalculateStatus lrs);
		//You don't need to calculate whole NN if only part of it is mutated

		public abstract void FindBPGradient(int test, float[] innerBPGradients, float[][] innerWeights);

		public abstract void FindBPGradient(int test, float desiredValue);

		public abstract void Mutate(float mutagen);

		public abstract void CorrectWeightsByBP(int test, float[][] input);

		public abstract void Demutate(float mutagen);

		public abstract float[][] GetValues(int test);

		public abstract float GetAnswer(int test);

		public abstract float[] AllBPGradients(int test);

		[JsonIgnore]
		public abstract float[][] AllWeights { get; }

		public abstract int WeightsCount { get; }

		public abstract void InitValues(int testsCount);
	}
}

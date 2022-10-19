namespace Library
{
	public static class Math2
	{
		public static double Interpolate(double from, double to, double actual)
		{
			return (actual - from) / (to - from);
		}

		public static float Max(float[] array)
		{
			float max = array[0];
			for (int i = 1; i < array.Length; i++)
				if (array[i] > max)
					max = array[i];
			return max;
		}

		public static float Min(float[] array)
		{
			float min = array[0];
			for (int i = 1; i < array.Length; i++)
				if (array[i] < min)
					min = array[i];
			return min;
		}

		public static float NormalDistribution(float scale, float centralization, float smoothing, Random rnd)
		{
			//actually this is not normal distribution, but who cares
			float x = rnd.NextSingle();
			float y = (MathF.Pow(x, centralization) + x * smoothing) / (smoothing + 1);
			int sign = rnd.Next(2) * 2 - 1;

			return sign * scale * y;
		}
	}
}

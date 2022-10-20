using static AbsurdMoneySimulations.Logger;

namespace Library
{
	public static class Math2
	{
		public static int Factorial(int number)
		{
			int n = 1;

			for (int i = 1; i <= number; i++)
				n *= i;

			return n; // returns 1 when number is 0
		}

		public static int Combinations0(int n, int k)
		{
			if (n < k || n < 1 || k < 1)
				return 0;
			if (n == k)
				return 1;
			if (k == 1)
				return n;
			return Combinations0(n - 1, k - 1) + Combinations0(n - 1, k);
		}

		public static int Combinations1(int n, int k)
		{
			if (n > 13)
				throw new ArgumentException();
			return Factorial(n) / (Factorial(k) * Factorial(n - k));
		}

		public static double Combinations2(int n, int k)
		{
			double ret = 1;
			while (k > 0)
			{
				ret = ret * n / k;
				k--; 
				n--;
			}
			return ret;
		}

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

		public static double CalculateRandomness(double k, int n, double p)
		{
			if (k == n / 2.0)
				return 1;
			else
			{
				double randomness;

				if (k <= n / 2.0)
					randomness = CumulativeDistributionFunction(k, n, 0.5);
				else
					randomness = CumulativeDistributionFunction(n - k, n, 0.5);

				return randomness * 2;
			}
		}

		public static float LikeNormalDistribution(float scale, float centralization, float smoothing, Random rnd)
		{
			//actually THIS is not normal distribution, but who cares
			float x = rnd.NextSingle();
			float y = (MathF.Pow(x, centralization) + x * smoothing) / (smoothing + 1);
			int sign = rnd.Next(2) * 2 - 1;

			return sign * scale * y;
		}

		public static double CumulativeDistributionFunction(double k, int n, double p)
		{
			var d = new MathNet.Numerics.Distributions.Binomial(p, n);
			return d.CumulativeDistribution(k);

			/*			double cdf = 0;
						for (int i = 0; i <= k; i++)
							cdf += d.Probability(i);

						return cdf;*/
		}
	}
}

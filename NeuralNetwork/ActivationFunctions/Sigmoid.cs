namespace AbsurdMoneySimulations
{
	public class Sigmoid : ActivationFunction
	{
		public override float f(float x)
		{
			return 1f / (1 + MathF.Pow(MathF.E, -x));
		}

		public override float df(float x)
		{
			float epx = MathF.Pow(MathF.E, -x);
			//return epx / (1 + epx)^2
			return epx / ((epx * epx + epx * 2 + 1));
			//return 1 / epx + 0.5f + epx;
		}
	}
}

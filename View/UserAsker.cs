namespace AbsurdMoneySimulations
{
	public static class UserAsker
	{
		public static bool Ask(string q)
		{
			return MessageBox.Show(q, "Hey", MessageBoxButtons.YesNo) == DialogResult.Yes;
		}

		public static void SayWait(string q)
		{
			MessageBox.Show(q);
		}
	}
}

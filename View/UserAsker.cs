namespace AbsurdMoneySimulations
{
	public static class UserAsker
	{
		public static bool Ask(string q)
		{
			DialogResult confirmResult = MessageBox.Show(q, "Hey", MessageBoxButtons.YesNo);
			if (confirmResult == DialogResult.Yes)
				return true;
			else
				return false;
		}

		public static void SayWait(string q)
		{
			DialogResult confirmResult = MessageBox.Show(q);
		}
	}
}

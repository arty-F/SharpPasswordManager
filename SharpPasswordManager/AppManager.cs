namespace SharpPasswordManager
{
	/// <summary>
	/// Gain access to entered by user password and calculates starting index for data storage.
	/// </summary>
    public static class AppManager
    {
		private static string password;
		public static string Password
		{
			get { return password; }
			set 
			{ 
				password = value;
				StartingIndex = int.Parse(password);
			}
		}

		public static int StartingIndex { get; private set; }

		public static int GetIndex(int i)
		{
			return i + StartingIndex;
		}
	}
}

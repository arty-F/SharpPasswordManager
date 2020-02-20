namespace SharpPasswordManager
{
	/// <summary>
	/// Gain access to entered by user password which is used as cryptography key and calculates starting index for data storage.
	/// </summary>
	public static class SecureManager
    {
		public static int StartingIndex { get; private set; }

		private static string key;
		public static string Key
		{
			get { return key; }
			set 
			{ 
				key = value;
				StartingIndex = int.Parse(key);
			}
		}

		public static int GetIndexOf(int i)
		{
			return i + StartingIndex;
		}
	}
}

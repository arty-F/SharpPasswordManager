namespace SharpPasswordManager.BL.Handlers
{
	public class SecureHandler : ISecureHandler
    {
		public string PasswordKey { get; private set; } = "Password";
		
		public string DataFileName { get; private set; } = "Data.bin";
		
		public string CategoriesFileName { get; private set; } = "Categories.bin";
		
		private int startingIndex;

		private string key;
		public string Key
		{
			get { return key; }
			set 
			{ 
				key = value;
				startingIndex = int.Parse(key);
			}
		}

		public int GetIndexOf(int i)
		{
			return i + startingIndex;
		}
	}
}

namespace SharpPasswordManager.BL.Handlers
{
    public class SecureHandler : ISecureHandler
    {
        public string PasswordKey { get; private set; } = "Password";

        public string DataFileName { get; private set; } = "Data.bin";

        public string CategoriesFileName { get; private set; } = "Categories.bin";

        private int startingIndex;

        private string secretKey;
        public string SecretKey
        {
            get { return secretKey; }
            set
            {
                secretKey = value;
                startingIndex = GetIntFromString(secretKey);
            }
        }

        public int GetIndexOf(int i)
        {
            return unchecked(i + startingIndex);
        }

        private int GetIntFromString(string str)
        {
            if (int.TryParse(str, out int result))
            {
                return result;
            }
            else
            {
                result = 0;

                foreach (var c in str)
                    result += c;

                return result;
            }
        }
    }
}

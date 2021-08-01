using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SharpPasswordManager.DL.DataGenerators
{
    /// <summary>
    /// Generate specific random data for models.
    /// </summary>
    public class DataGenerator : IDataGenerator
    {
        const string allowedСharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._ ";
        const string allowedСharactersNoSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private const int secondsInDay = 86400;
        private readonly int createdDateRangeInDays;
        private readonly int createdDateFutureMultiplier;
        private readonly int wordMinLength;
        private readonly int wordMaxLength;
        private readonly int passwordMinLength;
        private readonly int passwordMaxLength;
        Random random = new Random();

        /// <summary>
        /// Create a new instance of DataGenerator.
        /// </summary>
        /// <param name="createdDateRangeInDays">Range of randomly generated dates from now to (now + this value) in days.</param>
        /// <param name="wordMinLength">Generated characters minimal quantity. It used for generate logins and descriptions.</param>
        /// <param name="wordMaxLength">Generated characters maximum quantity. It used for generate logins and descriptions.</param>
        /// <param name="passwordMinLength">Generated characters minimal quantity. Used for generate passwords.</param>
        /// <param name="passwordMaxLength">Generated characters maximal quantity. Used for generate passwords.</param>
        public DataGenerator(int createdDateRangeInDays = 365, int wordMinLength = 4, int wordMaxLength = 20, int passwordMinLength = 6, int passwordMaxLength = 18, int createdDateFutureMultiplier = 10)
        {
            this.createdDateRangeInDays = createdDateRangeInDays;
            this.wordMinLength = wordMinLength;
            this.wordMaxLength = wordMaxLength;
            this.passwordMinLength = passwordMinLength;
            this.passwordMaxLength = passwordMaxLength;
            this.createdDateFutureMultiplier = createdDateFutureMultiplier;
        }

        /// <summary>
        /// Generate random datetime in sting format from now to to (now + this value) in days. Every ~100th generated date will be the long distant future.
        /// </summary>
        /// <returns>Random generated datetime string.</returns>
        public string GenerateRandomDate()
        {
            if (random.Next(100) == 99)
                return DateTime.Now.AddSeconds(random.Next(secondsInDay)).AddDays(random.Next(createdDateRangeInDays * createdDateFutureMultiplier)).ToString();
            else
                return DateTime.Now.AddSeconds(random.Next(secondsInDay)).AddDays(random.Next(createdDateRangeInDays)).ToString();
        }

        /// <summary>
        /// Generate random string whose lenght is in (minLenght : maxLenght) range.
        /// </summary>
        /// <returns>Random generated string.</returns>
        public string GenerateRandomUrl()
        {
            return GetRandomString(wordMinLength, wordMaxLength);
        }

        /// <summary>
        /// Generate random string of two types: one word string, and email address simulation string.
        /// </summary>
        /// <returns>Random generated string.</returns>
        public string GenerateRandomLogin()
        {
            string result = "";
            switch (random.Next(0, 2))
            {
                // One word login emulation
                case 0:
                {
                    result = GetRandomString(wordMinLength, wordMaxLength, allowedСharactersNoSymbols);
                    break;
                }
                // Email login emulation
                case 1:
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(GetRandomString(wordMinLength, wordMaxLength, allowedСharactersNoSymbols));
                    builder.Append("@");
                    builder.Append(GetRandomString(wordMinLength, wordMaxLength, allowedСharactersNoSymbols));
                    builder.Append(".com");
                    result = builder.ToString();
                    break;
                }
                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// Defines random number between minLenght and maxLenght. Then call <see cref="GenerateRandomPassword"/> method with defined random number as parameter and strongly random provider.
        /// </summary>
        /// <returns>Random generated password.</returns>
        public string GenerateRandomPassword()
        {
            int rndLength = random.Next(passwordMinLength, passwordMaxLength);
            return GenerateRandomPassword(rndLength);
        }

        /// <summary>
        /// Generate random string password with strongly random provider.
        /// </summary>
        /// <param name="length">Lenght of generated string.</param>
        /// <returns>Random generated password.</returns>
        public string GenerateRandomPassword(int length)
        {
            string result;
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[length];
                byte[] buffer = null;
                int maxRandom = byte.MaxValue - ((byte.MaxValue + 1) % allowedСharactersNoSymbols.Length);
                rng.GetBytes(data);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < length; i++)
                {
                    byte value = data[i];
                    while (value > maxRandom)
                    {
                        if (buffer == null)
                            buffer = new byte[1];
                        
                        rng.GetBytes(buffer);
                        value = buffer[0];
                    }
                    builder.Append(allowedСharactersNoSymbols[value % allowedСharactersNoSymbols.Length]);
                }
                result = builder.ToString();
            }
            return result;
        }

        /*----------------------------------------------------------------------------------------------------
         * Generate random string whose chars from parameter, and lenght is a random value between <minLenght> 
         and <maxLenght>.
        ----------------------------------------------------------------------------------------------------*/
        private string GetRandomString(int minLenght, int maxLenght, string allowedСhars = allowedСharacters)
        {
            int rndLenght = random.Next(minLenght, maxLenght);
            return new string(Enumerable.Repeat(allowedСhars, rndLenght).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

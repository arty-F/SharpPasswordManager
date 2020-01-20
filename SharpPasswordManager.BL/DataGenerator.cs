using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SharpPasswordManager.BL
{
    public class DataGenerator : IDataGenerator
    {
        const string allowedСharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._ ";
        const string allowedСharactersNoSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        /*----------------------------------------------------------------------------------------------------
         * Generate random date from <Now> to <Plus one year>.
        ----------------------------------------------------------------------------------------------------*/
        public DateTime GenerateRandomDate()
        {
            Random rng = new Random();
            int secondsInDay = 86400;
            int daysInYear = 365;
            return DateTime.Now.AddSeconds(rng.Next(secondsInDay)).AddDays(rng.Next(daysInYear));
        }

        /*----------------------------------------------------------------------------------------------------
         * Generate random string whose chars from <allowedСharacters> and lenght is in 
         ( <maxLenght> : <maxLenght> ) range.
        ----------------------------------------------------------------------------------------------------*/
        public string GenerateRandomDescription()
        {
            int minLength = 4;
            int maxLength = 20;
            return GetRandomString(minLength, maxLength);
        }

        /*----------------------------------------------------------------------------------------------------
         * Generate random string of two types: one word string, and email address simulation string.
        ----------------------------------------------------------------------------------------------------*/
        public string GenerateRandomLogin()
        {
            Random rng = new Random();
            string result = "";
            switch (rng.Next(0, 2))
            {
                // One word login emulation
                case 0:
                {
                    int minLength = 4;
                    int maxLength = 20;
                    result = GetRandomString(minLength, maxLength, allowedСharactersNoSymbols);
                    break;
                }
                // Email login emulation
                case 1:
                {
                    int minLength = 6;
                    int maxLength = 12;
                    StringBuilder builder = new StringBuilder();
                    builder.Append(GetRandomString(minLength, maxLength, allowedСharactersNoSymbols));
                    builder.Append("@");
                    builder.Append(GetRandomString(minLength, maxLength, allowedСharactersNoSymbols));
                    builder.Append(".com");
                    result = builder.ToString();
                    break;
                }
                default:
                    break;
            }
            return result;
        }

        /*----------------------------------------------------------------------------------------------------
         * Defines random number between <minLenght> and <maxLenght>. Then call <GenerateRandomPassword> mathod
         with defined random number.
        ----------------------------------------------------------------------------------------------------*/
        public string GenerateRandomPassword()
        {
            int minLength = 6;
            int maxLength = 18;
            Random rng = new Random();
            int rndLength = rng.Next(minLength, maxLength);
            return GenerateRandomPassword(rndLength);
        }

        /*----------------------------------------------------------------------------------------------------
         * Generate random string whose chars from <allowedСharactersNoSymbols> and lenght is a parameter.
        ----------------------------------------------------------------------------------------------------*/
        public string GenerateRandomPassword(int length)
        {
            string result = "";
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
                        {
                            buffer = new byte[1];
                        }
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
            Random rng = new Random();
            int rndLenght = rng.Next(minLenght, maxLenght);
            return new string(Enumerable.Repeat(allowedСhars, rndLenght).Select(s => s[rng.Next(s.Length)]).ToArray());
        }
    }
}

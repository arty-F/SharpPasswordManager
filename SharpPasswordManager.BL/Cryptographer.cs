using System;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using SharpPasswordManager.BL.Interfaces;

namespace SharpPasswordManager.BL
{
    /*-----------------------------------------------------------------------------
     * Class using Aes for cryptography. When encrypting the first <ivLength> bytes
     is non encrypted IV in encrypting result string. When decrypting, take first
     <ivLength> bytes and using them as IV, then getting remaining part of encrypted
     string, and decrypt it.
     * Key for cryptographe reciving in class constructor as parameter. If key length
     less than <keyLength> missing bytes will be appended with 0. If key length more
     than <keyLength> extra byte will be discarded.
    -----------------------------------------------------------------------------*/
    /// <summary>
    /// Used AesCryptoServiceProvider to encrypt/decrypt string data.
    /// </summary>
    public class Cryptographer : ICryptographer, IDisposable
    {
        private byte[] key;
        private readonly int ivLength = 16;
        private readonly int keyLength = 16;
        private bool disposed = false;

        /// <summary>
        /// Create a new class instance. Key for aes algoritm will be generated automatically.
        /// </summary>
        public Cryptographer()
        {
        }

        /// <summary>
        /// Create a new class instance with recieved key value.
        /// </summary>
        public Cryptographer(string key)
        {
            ChangeKey(key);
        }

        #region Disposing
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    for (int i = 0; i < key.Length; i++)
                        key[i] = 0;
                }
                disposed = true;
            }
        }

        ~Cryptographer()
        {
            Dispose(false);
        }
        #endregion

        /// <summary>
        /// Change key for aes alghoritm and makes it suitable length.
        /// </summary>
        /// <param name="newKey">New key.</param>
        public void ChangeKey(string newKey)
        {
            byte[] convertedKey = Convert.FromBase64String(EncodeTo64(newKey));
            byte[] requiredKey = new byte[keyLength];
            if (convertedKey.Length < keyLength)
            {
                for (int i = 0; i < keyLength; i++)
                {
                    if (i < convertedKey.Length)
                        requiredKey[i] = convertedKey[i];
                    else
                        requiredKey[i] = 0;
                }
            }
            else if (convertedKey.Length > keyLength)
            {
                for (int i = 0; i < keyLength; i++)
                    requiredKey[i] = convertedKey[i];
            }
            else
                requiredKey = convertedKey;

            key = requiredKey;
        }

        private string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(toEncode);
            return System.Convert.ToBase64String(toEncodeAsBytes);
        }

        /// <summary>
        /// Return encrypted by aes alghoritm string with IV.
        /// </summary>
        /// <param name="data">String for encryption.</param>
        public string Encypt(string data)
        {
            if (data == null)
                throw new ArgumentNullException("String");

            byte[] encryptedDataWithIV;
            byte[] encryptedData;
            byte[] iv = new byte[ivLength];
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                if (key != null)
                    aesAlg.Key = key;
                else
                    aesAlg.GenerateKey();

                aesAlg.GenerateIV();
                Array.Copy(sourceArray: aesAlg.IV, destinationArray: iv, length: ivLength);
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(data);
                        }
                        encryptedData = msEncrypt.ToArray();

                        encryptedDataWithIV = iv.Concat(encryptedData).ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encryptedDataWithIV);
        }

        /// <summary>
        /// Return decrypted by aes alghoritm string.
        /// </summary>
        /// <param name="data">Ecrypted string with IV.</param>
        public string Decrypt(string data)
        {
            if (data == null)
                throw new ArgumentNullException("String");

            string decrypted = null;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                byte[] encryptedBytesWithIV = Convert.FromBase64String(data);

                byte[] iv = new byte[ivLength];
                byte[] encryptedData = new byte[encryptedBytesWithIV.Length - iv.Length];

                Array.Copy(sourceArray: encryptedBytesWithIV, destinationArray: iv, length: ivLength);
                Array.Copy(sourceArray: encryptedBytesWithIV, sourceIndex: ivLength, destinationArray: encryptedData, destinationIndex: 0, length: encryptedData.Length);

                if (key != null)
                    aesAlg.Key = key;
                else
                    aesAlg.GenerateKey();

                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            decrypted = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return decrypted;
        }
    }
}

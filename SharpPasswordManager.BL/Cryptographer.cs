using System;
using System.Security.Cryptography;
using System.IO;

namespace SharpPasswordManager.BL
{
    /// <summary>
    /// Used AesCryptoServiceProvider to encrypt/decrypt string data.
    /// </summary>
    public class Cryptographer : ICryptographer, IDisposable
    {
        private readonly byte[] key;

        private bool disposed = false;

        public Cryptographer(byte[] key)
        {
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            this.key = key;
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
        /// Return encrypted by aes alghoritm string and iv.
        /// </summary>
        /// <param name="data">String for encryption.</param>
        public (string data, byte[] iv) Encypt(string data)
        {
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException("String");

            byte[] encrypted;
            byte[] iv;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV();
                iv = aesAlg.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(data);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return (Convert.ToBase64String(encrypted), iv);
        }

        /// <summary>
        /// Return decrypted by aes alghoritm string.
        /// </summary>
        /// <param name="cortege">Encrypted string and iv.</param>
        /// <returns></returns>
        public string Decrypt((string data, byte[] iv) cortege)
        {
            if (cortege.data == null || cortege.data.Length <= 0)
                throw new ArgumentNullException("String");

            string decrypted = null;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = cortege.iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cortege.data)))
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

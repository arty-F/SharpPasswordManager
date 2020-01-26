using System;
using System.Collections.Generic;
using System.Text;
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
        private readonly byte[] iv;

        private bool disposed = false;

        public Cryptographer(byte[] key, byte[] iv)
        {
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("IV");
            this.key = key;
            this.iv = iv;
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
                    
                    for (int i = 0; i < iv.Length; i++)
                        iv[i] = 0;
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
        /// Return decrypted by aes alghoritm string.
        /// </summary>
        /// <param name="str">Encrypted string.</param>
        /// <returns></returns>
        public string Decrypt(string str)
        {
            if (str == null || str.Length <= 0)
                throw new ArgumentNullException("String");

            string decrypted = null;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(str)))
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

        /// <summary>
        /// Return encrypted by aes alghoritm string.
        /// </summary>
        /// <param name="str">Encryption string.</param>
        /// <returns></returns>
        public string Encypt(string str)
        {
            if (str == null || str.Length <= 0)
                throw new ArgumentNullException("String");

            byte[] encrypted;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(str);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }
    }
}

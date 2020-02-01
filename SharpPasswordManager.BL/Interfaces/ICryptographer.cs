using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL
{
    /// <summary>
    /// Provides a mechanism to data encryption/decryption.
    /// </summary>
    public interface ICryptographer
    {
        string Encypt(string data);
        string Decrypt(string data);
        void ChangeKey(byte[] newKey);
    }
}

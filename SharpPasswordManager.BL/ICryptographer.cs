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
        (string data, byte[] iv) Encypt(string data);
        string Decrypt((string data, byte[] iv) cortege);
    }

    /// <summary>
    /// Used to determine the operating cryptographer mode.
    /// </summary>
    public enum CryptographyMode : byte
    {

        Encrypt = 0,
        Decrypt = 1
    }
}

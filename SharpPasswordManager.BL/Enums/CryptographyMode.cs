using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL.Enums
{
    /// <summary>
    /// Used to determine the operating cryptographer mode.
    /// </summary>
    public enum CryptographyMode : byte
    {
        Encrypt = 0,
        Decrypt = 1
    }
}

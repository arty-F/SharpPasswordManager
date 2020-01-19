using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL
{
    public interface IEncryptor
    {
        string Encypt(string str);
        string Decrypt(string str);
    }
}

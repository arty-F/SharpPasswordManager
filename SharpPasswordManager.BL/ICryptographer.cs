using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL
{
    public interface ICryptographer
    {
        string Encypt(string str);
        string Decrypt(string str);
    }
}

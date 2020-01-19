using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.DL
{
    public interface IStorage
    {
        string GetData(int id);
    }
}

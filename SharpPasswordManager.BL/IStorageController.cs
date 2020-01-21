using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL
{
    public interface IStorageController
    {
        string GetData(int index);
        void PasteData(int index);
        object GetCategories();
        object GetOptions();
    }
}

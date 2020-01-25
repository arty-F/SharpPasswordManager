using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL
{
    public interface IStorageController<TModel>
    {
        TModel Get(int index);
        void PasteAt(int index, TModel model);
    }

    public enum CryptographyMode : byte
    {
        Encrypt = 0,
        Decrypt = 1
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL
{
    public interface IStorageController
    {
        void Initialize();
        string GetData(int index);
        void PasteData(int index, int categoryId);
        void DeleteData(int index);
        object GetCategories();
        void PasteCategory();
        void DeleteCategory(int id);
        void RenameCategory(int id, string name);
    }
}

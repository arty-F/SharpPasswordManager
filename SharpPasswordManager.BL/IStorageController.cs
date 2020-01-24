using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL
{
    //TODO REWORK TO GENERIC
    public interface IStorageController
    {
        void Initialize();
        string GetData(int index);
        void PasteData(int index, int categoryId, string description, string login, DateTime date, string password);
        void DeleteData(int index);
        object GetCategories();
        void PasteCategory();
        void DeleteCategory(int id);
        void RenameCategory(int id, string name);
    }
}

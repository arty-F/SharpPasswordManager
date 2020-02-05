using System.Collections.Generic;

namespace SharpPasswordManager.BL.Interfaces
{
    /// <summary>
    /// Provides a mechanism to manage models collection.
    /// </summary>
    /// <typeparam name="TModel">Any model class.</typeparam>
    public interface IStorageController<TModel>
    {
        TModel Get(int index);
        void PasteAt(int index, TModel model);
        int Count();
        void CreateStorage(IEnumerable<TModel> modelsList);
    }
}

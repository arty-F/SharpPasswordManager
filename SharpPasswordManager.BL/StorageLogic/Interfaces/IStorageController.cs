using System.Collections.Generic;

namespace SharpPasswordManager.BL.StorageLogic
{
    /// <summary>
    /// Provides a mechanism to manage models collection.
    /// </summary>
    /// <typeparam name="TModel">Any model class.</typeparam>
    public interface IStorageController<TModel>
    {
        TModel Get(int index);
        void PasteAt(int index, TModel model);
        void Add(TModel model);
        void Remove(TModel model);
        int Count();
        List<TModel> ToList();
        void CreateStorage(IEnumerable<TModel> modelsList);
    }
}

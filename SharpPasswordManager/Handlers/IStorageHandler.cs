using System.Collections.Generic;

namespace SharpPasswordManager.Handlers
{
    public interface IStorageHandler<TCategory, TData>
    {
        TCategory CurrentCategory { get; set; }

        List<TCategory> GetCategories();
        List<TData> GetData();
        void RemoveCategory(TCategory category);
        void RemoveData(TData data);
        void AddCategory(TCategory category);
        void AddData(TData data);
        void ReplaceCategory(TCategory oldCategory, TCategory newCategory);
        void ReplaceData(TData oldData, TData newData);
    }
}

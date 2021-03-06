﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpPasswordManager.BL.StorageLogic
{
    /// <summary>
    /// Provides mechanism to manage categories storage controller and data storage controller.
    /// </summary>
    /// <typeparam name="TCategory"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public interface IMultipleStorageController<TCategory, TData>
    {
        List<TCategory> GetCategories();
        List<TData> GetData(TCategory ofCategory);
        void RemoveCategory(TCategory category);
        void RemoveData(TData data);
        void AddCategory(TCategory category);
        void AddData(TData data, TCategory toCategory);
        void ReplaceCategory(TCategory oldCategory, TCategory newCategory);
        void ReplaceData(TData oldData, TData newData);
        Task SecureStorageAsync();
    }
}

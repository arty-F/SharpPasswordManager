using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL
{
    /// <summary>
    /// Provides a mechanism to manage models collection.
    /// </summary>
    /// <typeparam name="TModel">Any model class.</typeparam>
    public interface IStorageController<TModel> where TModel : class
    {
        TModel Get(int index);
        void PasteAt(int index, TModel model);
        int Count();
    }
}

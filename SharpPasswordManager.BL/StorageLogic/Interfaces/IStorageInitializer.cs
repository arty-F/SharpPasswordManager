using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpPasswordManager.BL.StorageLogic
{
    /// <summary>
    /// Serves to generate random string and datetime prepertiy values for model.
    /// </summary>
    /// <typeparam name="TModel">Any model class.</typeparam>
    public interface IStorageInitializer<TModel>
    {
        /// <summary>
        /// Generate sequence of TModel with generated properties and return this collection.
        /// </summary>
        /// <param name="modelsCount">Required TModel count in list.</param>
        public List<TModel> GetData(int modelsCount);

        /// <summary>
        /// Asynchronously generate sequence of TModel with generated properties and return this collection.
        /// </summary>
        /// <param name="modelsCount">Required TModel count in list.</param>
        public Task<List<TModel>> GetDataAsync(int modelsCount);
    }
}

using SharpPasswordManager.BL.Interfaces;
using SharpPasswordManager.DL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpPasswordManager.Handlers
{
    /// <summary>
    /// Manage two <seealso cref="IStorageController{TModel}"/> instance, one of which storage categories and second storage data.
    /// </summary>
    public class StorageHandler : IStorageHandler<CategoryModel, DataModel>
    {
        private IStorageController<CategoryModel> categoryController;
        private IStorageController<DataModel> dataController;

        private Dictionary<DataModel, int> usingData = new Dictionary<DataModel, int>();
        /// <summary>
        /// Selected model of <see cref="CategoryModel"/>.
        /// </summary>
        public int CurrentCategoryIndex { get; set; } = -1;

        private Random random = new Random();

        /// <summary>
        /// Create a new class instance.
        /// </summary>
        /// <param name="categoryController"><seealso cref="IStorageController{TModel}"/> with categories.</param>
        /// <param name="dataController"><seealso cref="IStorageController{TModel}"/> with data.</param>
        public StorageHandler(IStorageController<CategoryModel> categoryController, IStorageController<DataModel> dataController)
        {
            this.categoryController = categoryController;
            this.dataController = dataController;
        }

        /// <summary>
        /// Add <seealso cref="CategoryModel"/> to category storage contoller.
        /// </summary>
        /// <param name="category">Adding category</param>
        public void AddCategory(CategoryModel category)
        {
            categoryController.Add(category);
        }

        /// <summary>
        /// Write <seealso cref="DataModel"/> to data storage contoller at defined random index. Then write that index to CurrentCategory index collection, and save changes of category storage controller.
        /// </summary>
        /// <param name="category">Adding data</param>
        public void AddData(DataModel data)
        {
            List<int> usingIndexes = new List<int>();
            for (int i = 0; i < categoryController.Count(); i++)
            {
                foreach (var index in categoryController.Get(i).DataIndexes)
                {
                    usingIndexes.Add(index);
                }
            }

            int maxValue = dataController.Count();
            int newIndex = random.Next(0, maxValue);
            while (usingIndexes.Contains(newIndex))
                newIndex = random.Next(0, maxValue);

            dataController.PasteAt(SecureManager.GetIndexOf(newIndex), data);

            CategoryModel category = categoryController.Get(CurrentCategoryIndex);
            category.DataIndexes.Add(newIndex);
            categoryController.PasteAt(CurrentCategoryIndex, category);
        }

        /// <summary>
        /// Get list of categories.
        /// </summary>
        /// <returns></returns>
        public List<CategoryModel> GetCategories()
        {
            return categoryController.ToList();
        }

        /// <summary>
        /// Get list of data referenced in CurrentCategory.
        /// </summary>
        /// <returns></returns>
        public List<DataModel> GetData()
        {
            List<DataModel> dataList = new List<DataModel>();
            CategoryModel currentCategory = categoryController.Get(CurrentCategoryIndex);
            dataList.Capacity = currentCategory.DataIndexes.Count();

            foreach (var index in currentCategory.DataIndexes)
            {
                DataModel data = dataController.Get(SecureManager.GetIndexOf(index));
                dataList.Add(data);
            }

            SetUsingData(currentCategory.DataIndexes);

            return dataList;
        }

        /// <summary>
        /// Remove category from cetegory storage controller.
        /// </summary>
        /// <param name="category">Category to remove.</param>
        public void RemoveCategory(CategoryModel category)
        {
            categoryController.Remove(category);
        }

        /// <summary>
        /// Finds which category refers to this data, and remove this reference.
        /// </summary>
        /// <param name="data">Data to remove.</param>
        public void RemoveData(DataModel data)
        {
            int index = -1;
            foreach (var item in usingData)
            {
                if (item.Key.Equals(data))
                {
                    index = item.Value;
                    break;
                }
            }

            if (index > -1)
            {
                CategoryModel currentCategory = categoryController.Get(CurrentCategoryIndex);
                currentCategory.DataIndexes.Remove(index);
                categoryController.PasteAt(CurrentCategoryIndex, currentCategory);
                SetUsingData(currentCategory.DataIndexes);
            }
        }

        /// <summary>
        /// Replace old category by new.
        /// </summary>
        public void ReplaceCategory(CategoryModel oldCategory, CategoryModel newCategory)
        {
            for (int i = 0; i < categoryController.Count(); i++)
            {
                if (categoryController.Get(i).Equals(oldCategory))
                {
                    categoryController.PasteAt(i, newCategory);
                    break;
                }
            }
        }

        /// <summary>
        /// Replace old data by new.
        /// </summary>
        public void ReplaceData(DataModel oldData, DataModel newData)
        {
            foreach (var item in usingData)
            {
                if (item.Key.Equals(oldData))
                {
                    dataController.PasteAt(SecureManager.GetIndexOf(item.Value), newData);
                    SetUsingData(usingData.Values.ToList());
                    break;
                }
            }
        }

        private void SetUsingData(List<int> indexes)
        {
            usingData.Clear();
            foreach (var index in indexes)
            {
                usingData.Add(dataController.Get(SecureManager.GetIndexOf(index)), index);
            }
        }
    }
}

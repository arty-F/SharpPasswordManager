using SharpPasswordManager.BL.Handlers;
using SharpPasswordManager.DL.DataGenerators;
using SharpPasswordManager.DL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpPasswordManager.BL.StorageLogic
{
    /// <summary>
    /// Manage two <seealso cref="IStorageController{TModel}"/> instance, one of which storage categories and second storage data.
    /// </summary>
    public class MultipleStorageController : IMultipleStorageController<CategoryModel, DataModel>
    {
        private IStorageController<CategoryModel> categoryController;
        private IStorageController<DataModel> dataController;
        private ISecureHandler secureHandler;

        private Random random = new Random();

        /// <summary>
        /// Create a new class instance.
        /// </summary>
        /// <param name="categoryController"><seealso cref="IStorageController{TModel}"/> with categories.</param>
        /// <param name="dataController"><seealso cref="IStorageController{TModel}"/> with data.</param>
        public MultipleStorageController(IStorageController<CategoryModel> categoryController, IStorageController<DataModel> dataController, ISecureHandler secureHandler)
        {
            this.categoryController = categoryController;
            this.dataController = dataController;
            this.secureHandler = secureHandler;
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
        public void AddData(DataModel data, CategoryModel toCategory)
        {
            List<int> usingIndexes = GetUsingDataIndexes();

            int maxValue = dataController.Count();
            int newIndex = random.Next(0, maxValue);
            while (usingIndexes.Contains(newIndex))
                newIndex = random.Next(0, maxValue);

            dataController.PasteAt(secureHandler.GetIndexOf(newIndex), data);

            List<CategoryModel> categories = categoryController.ToList();

            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].Equals(toCategory))
                {
                    CategoryModel newCategory = new CategoryModel { DataIndexes = new List<int>(toCategory.DataIndexes), Name = toCategory.Name };
                    newCategory.DataIndexes.Add(newIndex);
                    categoryController.PasteAt(i, newCategory);
                    break;
                }
            }
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
        public List<DataModel> GetData(CategoryModel ofCategory)
        {
            List<DataModel> dataList = new List<DataModel>();

            if (ofCategory.DataIndexes.Count == 0)
                return dataList;

            dataList.Capacity = ofCategory.DataIndexes.Count;
            foreach (var dataIndex in ofCategory.DataIndexes)
            {
                dataList.Add(dataController.Get(secureHandler.GetIndexOf(dataIndex)));
            }

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
            List<CategoryModel> categories = GetCategories();
            for (int i = 0; i < categories.Count; i++)
            {
                foreach (var dataIndex in categories[i].DataIndexes)
                {
                    if (dataController.Get(secureHandler.GetIndexOf(dataIndex)).Equals(data))
                    {
                        CategoryModel newCategory = new CategoryModel { DataIndexes = new List<int>(categories[i].DataIndexes), Name = categories[i].Name };
                        newCategory.DataIndexes.Remove(dataIndex);
                        categoryController.PasteAt(i, newCategory);
                        return;
                    }
                }
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
            List<int> usingIndexes = GetUsingDataIndexes();

            foreach (var index in usingIndexes)
            {
                if (dataController.Get(secureHandler.GetIndexOf(index)).Equals(oldData))
                {
                    dataController.PasteAt(secureHandler.GetIndexOf(index), newData);
                    break;
                }
            }
        }

        public async Task SecureStorageAsync()
        {
            await Task.Run(SecureStorage);
        }

        private void SecureStorage()
        {
            var dataIndexes = GetUsingDataIndexes();
            if (dataIndexes.Count == 0)
                return;

            List<DataModel> data = new List<DataModel>();
            foreach (var index in dataIndexes)
                data.Add(dataController.Get(secureHandler.GetIndexOf(index)));

            Random rng = new Random();
            DataGenerator generator = new DataGenerator();
            int storageLength = dataController.Count();
            foreach (var item in data)
            {
                for (int i = 0; i < storageLength / (50 * data.Count); i++)
                {
                    int index;
                    do
                    {
                        index = rng.Next(storageLength);
                    } while (dataIndexes.Contains(index));

                    dataController.PasteAt(secureHandler.GetIndexOf(index),
                        new DataModel
                        {
                            Date = item.Date,
                            Url = item.Url,
                            Login = item.Login,
                            Password = generator.GenerateRandomPassword(item.Password.Length)
                        });
                }
            }
        }

        private List<int> GetUsingDataIndexes()
        {
            List<int> usingIndexes = new List<int>();
            List<CategoryModel> categories = categoryController.ToList();
            foreach (var category in categories)
            {
                foreach (var index in category.DataIndexes)
                {
                    usingIndexes.Add(index);
                }
            }
            return usingIndexes;
        }
    }
}

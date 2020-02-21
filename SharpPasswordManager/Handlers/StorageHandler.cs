using SharpPasswordManager.BL.Interfaces;
using SharpPasswordManager.DL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpPasswordManager.Handlers
{
    public class StorageHandler : IStorageHandler<CategoryModel, DataModel>
    {
        private IStorageController<CategoryModel> categoryController;
        private IStorageController<DataModel> dataController;

        private Dictionary<DataModel, int> usingData = new Dictionary<DataModel, int>();
        public CategoryModel CurrentCategory { get; set; }
        Random random = new Random();

        public StorageHandler(IStorageController<CategoryModel> categoryController, IStorageController<DataModel> dataController)
        {
            this.categoryController = categoryController;
            this.dataController = dataController;
        }

        public void AddCategory(CategoryModel category)
        {
            categoryController.Add(category);
        }

        public void AddData(DataModel data)
        {
            if (CurrentCategory == null)
            {
                return;
            }

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

            for (int i = 0; i < categoryController.Count(); i++)
            {
                CategoryModel category = categoryController.Get(i);
                if (category.Equals(CurrentCategory))
                {
                    category.DataIndexes.Add(newIndex);
                    categoryController.PasteAt(i, category);
                    break;
                }
            }
        }

        public List<CategoryModel> GetCategories()
        {
            return categoryController.ToList();
        }

        public List<DataModel> GetData()
        {
            if (CurrentCategory != null)
            {
                List<DataModel> dataList = new List<DataModel>();
                dataList.Capacity = CurrentCategory.DataIndexes.Count();

                foreach (var index in CurrentCategory.DataIndexes)
                {
                    DataModel data = dataController.Get(SecureManager.GetIndexOf(index));
                    dataList.Add(data);
                }

                SetUsingData(CurrentCategory.DataIndexes);

                return dataList;
            }
            else
                return new List<DataModel>();
        }

        public void RemoveCategory(CategoryModel category)
        {
            categoryController.Remove(category);
        }

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
                for (int i = 0; i < categoryController.Count(); i++)
                {
                    CategoryModel category = categoryController.Get(i);
                    if (category.DataIndexes.Contains(index))
                    {
                        category.DataIndexes.Remove(index);
                        categoryController.PasteAt(i, category);
                        SetUsingData(category.DataIndexes);
                        break;
                    }
                }
            }
        }

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

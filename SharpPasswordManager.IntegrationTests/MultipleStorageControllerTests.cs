using NUnit.Framework;
using SharpPasswordManager.BL.Handlers;
using SharpPasswordManager.BL.Security;
using SharpPasswordManager.BL.StorageLogic;
using SharpPasswordManager.DL.DataGenerators;
using SharpPasswordManager.DL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SharpPasswordManager.IntegrationTests
{
    public class MultipleStorageControllerTests
    {
        const int modelsCount = 100;

        string categoryPath;

        string modelPath;

        IMultipleStorageController<CategoryModel, DataModel> multipleStorageController;

        IStorageController<DataModel> modelController;

        IStorageController<CategoryModel> categoryController;

        List<DataModel> data;

        ICryptographer cryptographer;

        ISecureHandler secureHandler;

        public MultipleStorageControllerTests()
        {
            var key = "1234567890asdfghjkl";
            cryptographer = new Cryptographer(key);

            secureHandler = new SecureHandler();

            secureHandler.SecretKey = "55";

            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            categoryPath = Path.Combine(assemblyPath, secureHandler.CategoriesFileName);

            modelPath = Path.Combine(assemblyPath, secureHandler.DataFileName);

            var storageInitializer = new StorageInitializer<DataModel>(new DataGenerator(), cryptographer);

            data = storageInitializer.GetData(modelsCount);
        }

        [SetUp]
        public void Setup()
        {
            ClearFiles();

            categoryController = new StorageController<CategoryModel>(categoryPath, cryptographer);

            categoryController.CreateStorage(new List<CategoryModel>());

            modelController = new StorageController<DataModel>(modelPath, cryptographer);

            modelController.CreateStorage(data);

            multipleStorageController = new MultipleStorageController(categoryController, modelController, secureHandler);
        }

        private void ClearFiles()
        {
            if (File.Exists(categoryPath))
                File.Delete(categoryPath);

            if (File.Exists(modelPath))
                File.Delete(modelPath);
        }

        [Test]
        public void Storages_is_clear_at_tests_start()
        {
            Assert.That(multipleStorageController.GetCategories().Count, Is.EqualTo(0));
        }

        [Test]
        public void AddCategory_is_adds_one()
        {
            var category = new CategoryModel { Name = "1234", DataIndexes = new List<int>() };

            multipleStorageController.AddCategory(category);

            Assert.That(multipleStorageController.GetCategories().Count, Is.EqualTo(1));
        }

        [Test]
        public void AddCategory_work_at_same_name_category()
        {
            var category = new CategoryModel { Name = "1234", DataIndexes = new List<int>() };

            multipleStorageController.AddCategory(category);
            multipleStorageController.AddCategory(category);

            Assert.That(multipleStorageController.GetCategories().Count, Is.EqualTo(2));
        }

        [Test]
        public void RemoveCategory_is_removing_one()
        {
            var category = new CategoryModel { Name = "1234", DataIndexes = new List<int>() };

            multipleStorageController.AddCategory(category);
            multipleStorageController.RemoveCategory(category);

            Assert.That(multipleStorageController.GetCategories().Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveCategory_is_removing_right_category()
        {
            var category = new CategoryModel { Name = "1234", DataIndexes = new List<int>() };
            var categoryToRem = new CategoryModel { Name = "abcd", DataIndexes = new List<int>() };

            multipleStorageController.AddCategory(category);
            multipleStorageController.AddCategory(categoryToRem);
            multipleStorageController.RemoveCategory(categoryToRem);

            var categories = multipleStorageController.GetCategories();

            Assert.Multiple(() =>
            {
                Assert.That(categories.Contains(category), Is.True);
                Assert.That(multipleStorageController.GetCategories().Count, Is.EqualTo(1));
            });
        }

        [Test]
        public void RemoveCategory_unexisted_dont_throw_ex()
        {
            var category = new CategoryModel { Name = "1234", DataIndexes = new List<int>() };

            Assert.DoesNotThrow(() => multipleStorageController.RemoveCategory(category));
        }

        [Test]
        public void AddData_is_adding_data()
        {
            var category = new CategoryModel { Name = "first", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(category);

            var dataModel = new DataModel { Date = DateTime.Now.ToString(), Login = "Log", Password = "Pass", Url = "Url" };
            multipleStorageController.AddData(dataModel, category);

            category = multipleStorageController.GetCategories().First(c => c.Name == category.Name);
            var data = multipleStorageController.GetData(category);

            Assert.That(data.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddData_can_add_to_different_categories()
        {
            var firstCategory = new CategoryModel { Name = "first", DataIndexes = new List<int>() };
            var secondCategory = new CategoryModel { Name = "second", DataIndexes = new List<int>() };

            multipleStorageController.AddCategory(firstCategory);
            multipleStorageController.AddCategory(secondCategory);

            var firstData = new DataModel { Date = DateTime.Now.ToString(), Login = "fLog", Password = "fPass", Url = "fUrl" };
            multipleStorageController.AddData(firstData, firstCategory);

            var secondData = new DataModel { Date = DateTime.Now.ToString(), Login = "sLog", Password = "sPass", Url = "sUrl" };
            multipleStorageController.AddData(secondData, secondCategory);

            firstCategory = multipleStorageController.GetCategories().First(c => c.Name == firstCategory.Name);
            secondCategory = multipleStorageController.GetCategories().First(c => c.Name == secondCategory.Name);

            var actualFirstData = multipleStorageController.GetData(firstCategory).First();
            var actualSecondData = multipleStorageController.GetData(secondCategory).First();

            Assert.Multiple(() => {
                Assert.That(actualFirstData, Is.EqualTo(firstData));
                Assert.That(actualSecondData, Is.EqualTo(secondData));
            });
        }

        [Test]
        public void AddData_can_add_multiple_data()
        {
            var category = new CategoryModel { Name = "cat", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(category);

            var firstData = new DataModel { Date = DateTime.Now.ToString(), Login = "fLog", Password = "fPass", Url = "fUrl" };
            multipleStorageController.AddData(firstData, category);

            var secondData = new DataModel { Date = DateTime.Now.ToString(), Login = "sLog", Password = "sPass", Url = "sUrl" };
            category = multipleStorageController.GetCategories().First(c => c.Name == category.Name);
            multipleStorageController.AddData(secondData, category);

            category = multipleStorageController.GetCategories().First(c => c.Name == category.Name);
            var actualData = multipleStorageController.GetData(category);

            Assert.That(actualData.Count, Is.EqualTo(2));
        }

        [Test]
        public void AddData_can_add_same_data()
        {
            var category = new CategoryModel { Name = "cat", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(category);

            var data = new DataModel { Date = DateTime.Now.ToString(), Login = "fLog", Password = "fPass", Url = "fUrl" };
            multipleStorageController.AddData(data, category);

            category = multipleStorageController.GetCategories().First(c => c.Name == category.Name);
            multipleStorageController.AddData(data, category);

            category = multipleStorageController.GetCategories().First(c => c.Name == category.Name);
            var actualData = multipleStorageController.GetData(category);

            Assert.That(actualData.Count, Is.EqualTo(2));
        }

        [Test]
        public void AddData_adding_correct_data()
        {
            var category = new CategoryModel { Name = "cat", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(category);

            var firstData = new DataModel { Date = DateTime.Now.ToString(), Login = "Log", Password = "Pass", Url = "Url" };
            multipleStorageController.AddData(firstData, category);

            var secondData = new DataModel { Date = DateTime.Now.ToString(), Login = "sLog", Password = "sPass", Url = "sUrl" };
            category = multipleStorageController.GetCategories().First(c => c.Name == category.Name);
            multipleStorageController.AddData(secondData, category);

            category = multipleStorageController.GetCategories().First(c => c.Name == category.Name);
            var actualData = multipleStorageController.GetData(category);

            Assert.Multiple(() => {
                Assert.That(actualData.Contains(firstData));
                Assert.That(actualData.Contains(secondData));
            });
        }

        [Test]
        public void RemoveData_is_removing_data()
        {
            var category = new CategoryModel { Name = "cat", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(category);

            var data = new DataModel { Date = DateTime.Now.ToString(), Login = "Log", Password = "Pass", Url = "Url" };
            multipleStorageController.AddData(data, category);

            multipleStorageController.RemoveData(data);

            category = multipleStorageController.GetCategories().First(c => c.Name == category.Name);
            var actualData = multipleStorageController.GetData(category);

            Assert.That(actualData.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveData_unexisted_data_dont_throw_ex()
        {
            var category = new CategoryModel { Name = "cat", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(category);

            var data = new DataModel { Date = DateTime.Now.ToString(), Login = "Log", Password = "Pass", Url = "Url" };

            Assert.DoesNotThrow(() => multipleStorageController.RemoveData(data));
        }

        [Test]
        public void RemoveData_unexisted_data_unexisted_category_dont_throw_ex()
        {
            var data = new DataModel { Date = DateTime.Now.ToString(), Login = "Log", Password = "Pass", Url = "Url" };

            Assert.DoesNotThrow(() => multipleStorageController.RemoveData(data));
        }

        [Test]
        public void ReplaceCategory_is_correctly_replace()
        {
            var category = new CategoryModel { Name = "1234", DataIndexes = new List<int>() };
            var categoryToRep = new CategoryModel { Name = "abcd", DataIndexes = new List<int>() };

            multipleStorageController.AddCategory(category);
            multipleStorageController.ReplaceCategory(category, categoryToRep);

            var categories = multipleStorageController.GetCategories();

            Assert.Multiple(() =>
            {
                Assert.That(categories.Count, Is.EqualTo(1));
                Assert.That(categories.Contains(categoryToRep), Is.True);
            });
        }

        [Test]
        public void ReplaceCategory_unexisted_dont_change_anything()
        {
            var category = new CategoryModel { Name = "1234", DataIndexes = new List<int>() };
            var categoryToRep = new CategoryModel { Name = "abcd", DataIndexes = new List<int>() };
            var unexistedCategory = new CategoryModel { Name = "abcd1234", DataIndexes = new List<int>() };

            multipleStorageController.AddCategory(category);
            multipleStorageController.ReplaceCategory(unexistedCategory, categoryToRep);

            var categories = multipleStorageController.GetCategories();

            Assert.Multiple(() =>
            {
                Assert.That(categories.Count, Is.EqualTo(1));
                Assert.That(categories.Contains(category), Is.True);
            });
        }

        [Test]
        public void ReplaceCategory_enexisted_dont_throw_ex()
        {
            var category = new CategoryModel { Name = "1234", DataIndexes = new List<int>() };
            var categoryToRep = new CategoryModel { Name = "abcd", DataIndexes = new List<int>() };

            Assert.DoesNotThrow(() => multipleStorageController.ReplaceCategory(category, categoryToRep));
        }

        [Test]
        public void ReplaceData_is_correctly_replace()
        {
            var category = new CategoryModel { Name = "cat", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(category);

            var data = new DataModel { Date = DateTime.Now.ToString(), Login = "fLog", Password = "fPass", Url = "fUrl" };
            multipleStorageController.AddData(data, category);

            var newData = new DataModel { Date = DateTime.Now.ToString(), Login = "newLog", Password = "newPass", Url = "newUrl" };
            multipleStorageController.ReplaceData(data, newData);

            category = multipleStorageController.GetCategories().First(c => c.Name == category.Name);
            var actualData = multipleStorageController.GetData(category);

            Assert.Multiple(() =>
            {
                Assert.That(actualData.Count, Is.EqualTo(1));
                Assert.That(actualData.Contains(newData), Is.True);
            });
        }

        [Test]
        public void ReplaceData_is_change_correct_category_and_data()
        {
            var category = new CategoryModel { Name = "cat", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(category);
            var categoryUnchanged = new CategoryModel { Name = "catUnchanged", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(categoryUnchanged);

            var dataUnchanged1 = new DataModel { Date = DateTime.Now.ToString(), Login = "fLogUnc1", Password = "fPassUnc1", Url = "fUrlUnc1" };
            multipleStorageController.AddData(dataUnchanged1, categoryUnchanged);
            categoryUnchanged = multipleStorageController.GetCategories().First(c => c.Name == categoryUnchanged.Name);
            var dataUnchanged2 = new DataModel { Date = DateTime.Now.ToString(), Login = "fLogUnc2", Password = "fPassUnc2", Url = "fUrlUnc2" };
            multipleStorageController.AddData(dataUnchanged2, categoryUnchanged);

            var data1 = new DataModel { Date = DateTime.Now.ToString(), Login = "Log1", Password = "Pass1", Url = "Url1" };
            multipleStorageController.AddData(data1, category);
            category = multipleStorageController.GetCategories().First(c => c.Name == category.Name);
            var data2 = new DataModel { Date = DateTime.Now.ToString(), Login = "Log2", Password = "Pass2", Url = "Url2" };
            multipleStorageController.AddData(data2, category);

            var newData = new DataModel { Date = DateTime.Now.ToString(), Login = "newLog", Password = "newPass", Url = "newUrl" };
            multipleStorageController.ReplaceData(data1, newData);

            var categories = multipleStorageController.GetCategories();
            categoryUnchanged = multipleStorageController.GetCategories().First(c => c.Name == categoryUnchanged.Name);
            category = multipleStorageController.GetCategories().First(c => c.Name == category.Name);

            var actualDataUnchanged = multipleStorageController.GetData(categoryUnchanged);
            var actualData = multipleStorageController.GetData(category);

            Assert.Multiple(() =>
            {
                Assert.That(categories.Count, Is.EqualTo(2));

                Assert.That(actualDataUnchanged.Count, Is.EqualTo(2));
                Assert.That(actualDataUnchanged.Contains(dataUnchanged1), Is.True);
                Assert.That(actualDataUnchanged.Contains(dataUnchanged2), Is.True);

                Assert.That(actualData.Count, Is.EqualTo(2));
                Assert.That(actualData.Contains(newData), Is.True);
                Assert.That(actualData.Contains(data2), Is.True);
            });
        }

        [Test]
        public void ReplaceData_unexisted_dont_throw()
        {
            var category = new CategoryModel { Name = "cat", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(category);

            var data = new DataModel { Date = DateTime.Now.ToString(), Login = "fLog", Password = "fPass", Url = "fUrl" };
            var newData = new DataModel { Date = DateTime.Now.ToString(), Login = "newLog", Password = "newPass", Url = "newUrl" };

            Assert.DoesNotThrow(() => multipleStorageController.ReplaceData(data, newData));
        }

        [Test]
        public void SecureDataAsync_is_not_throw_ex()
        {
            var category = new CategoryModel { Name = "cat1", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(category);
           
            var data = new DataModel { Date = DateTime.Now.ToString(), Login = "fLogUnc1", Password = "fPassUnc1", Url = "fUrlUnc1" };
            multipleStorageController.AddData(data, category);

            Assert.DoesNotThrowAsync(async () => await multipleStorageController.SecureStorageAsync());
        }

        [Test]
        public async Task SecureDataAsync_dont_broke_data()
        {
            var firstCategory = new CategoryModel { Name = "cat1", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(firstCategory);
            var secondCategory = new CategoryModel { Name = "cat2", DataIndexes = new List<int>() };
            multipleStorageController.AddCategory(secondCategory);

            var firstData1 = new DataModel { Date = DateTime.Now.ToString(), Login = "fLogUnc1", Password = "fPassUnc1", Url = "fUrlUnc1" };
            multipleStorageController.AddData(firstData1, firstCategory);
            firstCategory = multipleStorageController.GetCategories().First(c => c.Name == firstCategory.Name);
            var firstData2 = new DataModel { Date = DateTime.Now.ToString(), Login = "fLogUnc2", Password = "fPassUnc2", Url = "fUrlUnc2" };
            multipleStorageController.AddData(firstData2, firstCategory);

            var secondData1 = new DataModel { Date = DateTime.Now.ToString(), Login = "Log1", Password = "Pass1", Url = "Url1" };
            multipleStorageController.AddData(secondData1, secondCategory);
            secondCategory = multipleStorageController.GetCategories().First(c => c.Name == secondCategory.Name);
            var secondData2 = new DataModel { Date = DateTime.Now.ToString(), Login = "Log2", Password = "Pass2", Url = "Url2" };
            multipleStorageController.AddData(secondData2, secondCategory);

            await multipleStorageController.SecureStorageAsync();

            var categories = multipleStorageController.GetCategories();
            firstCategory = multipleStorageController.GetCategories().First(c => c.Name == firstCategory.Name);
            secondCategory = multipleStorageController.GetCategories().First(c => c.Name == secondCategory.Name);

            var firstCategoryData = multipleStorageController.GetData(firstCategory);
            var secondCategoryData = multipleStorageController.GetData(secondCategory);

            Assert.Multiple(() =>
            {
                Assert.That(categories.Count, Is.EqualTo(2));

                Assert.That(firstCategoryData.Count, Is.EqualTo(2));
                Assert.That(firstCategoryData.Contains(firstData1), Is.True);
                Assert.That(firstCategoryData.Contains(firstData2), Is.True);

                Assert.That(secondCategoryData.Count, Is.EqualTo(2));
                Assert.That(secondCategoryData.Contains(secondData1), Is.True);
                Assert.That(secondCategoryData.Contains(secondData2), Is.True);
            });
        }
    }
}

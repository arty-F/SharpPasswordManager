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

        private void ClearFiles()
        {
            if (File.Exists(categoryPath))
                File.Delete(categoryPath);

            if (File.Exists(modelPath))
                File.Delete(modelPath);
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



        /*[Test]
        public void RemoveCategory_is_removing_one()
        {
            CategoryModel firstCategory = new CategoryModel { Name = "first", DataIndexes = new List<int>() };
            CategoryModel secondCategory = new CategoryModel { Name = "second", DataIndexes = new List<int>() };

            multipleStorageController.AddCategory(firstCategory);
            multipleStorageController.AddCategory(secondCategory);

            Assert.That(multipleStorageController.GetCategories().Count, Is.EqualTo(0));
        }*/
    }
}

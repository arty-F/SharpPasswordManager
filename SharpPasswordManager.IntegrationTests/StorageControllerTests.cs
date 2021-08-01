using NUnit.Framework;
using SharpPasswordManager.BL.Security;
using SharpPasswordManager.BL.StorageLogic;
using SharpPasswordManager.DL.DataGenerators;
using SharpPasswordManager.IntegrationTests.Mocks;
using System;
using System.Collections.Generic;

namespace SharpPasswordManager.IntegrationTests
{
    public class StorageControllerTests
    {
        const int modelsCount = 10;

        IStorageController<ModelMock> controller;

        List<ModelMock> data;

        ICryptographer cryptographer;

        [SetUp]
        public void Setup()
        {
            string key = "1234567890asdfghjkl";
            cryptographer = new Cryptographer(key);

            var storageInitializer = new StorageInitializer<ModelMock>(new DataGenerator(), cryptographer);

            data = storageInitializer.GetData(modelsCount);

            controller = new StorageController<ModelMock>(data, cryptographer);
        }

        [Test]
        public void Count_is_equal_models_count()
        {
            int expected = modelsCount;
            int actual = controller.Count();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void PasteAt_model_pasted_at_right_index()
        {
            int index = 2;
            string expectedDate = new DateTime(2011, 11, 11).ToString();
            string expectedLogin = "Login";
            ModelMock mockModel = new ModelMock { Date = expectedDate, Login = expectedLogin };

            controller.PasteAt(index, mockModel);
            ModelMock resultModel = controller.Get(index);

            Assert.Multiple(() =>
            {
                Assert.That(resultModel.Date, Is.EqualTo(expectedDate));
                Assert.That(resultModel.Login, Is.EqualTo(expectedLogin));
            });
        }

        [Test]
        public void PasteAt_out_index_not_thrown_error()
        {
            int index = modelsCount + 10;
            string expectedDate = new DateTime(2011, 11, 11).ToString();
            string expectedLogin = "Login";
            ModelMock mockModel = new ModelMock { Date = expectedDate, Login = expectedLogin };

            controller.PasteAt(index, mockModel);
            ModelMock resultModel = controller.Get(index);

            Assert.Multiple(() =>
            {
                Assert.That(resultModel.Date, Is.EqualTo(expectedDate));
                Assert.That(resultModel.Login, Is.EqualTo(expectedLogin));
            });
        }

        [Test]
        public void Get_getting_correct_model()
        {
            var expected = cryptographer.Decrypt(data[0].Login);
            var actual = controller.Get(0).Login;

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Add_is_adding_the_model_to_next_index()
        {
            var newModel = new ModelMock
            {
                Login = "log1n",
                Date = DateTime.Now.ToString()
            };
            var expected = newModel.Login;

            controller.Add(newModel);
            var actual = controller.Get(modelsCount).Login;

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Count_is_increase_after_Adding()
        {
            int expected = modelsCount + 1;

            controller.Add(new ModelMock { Login = "123", Date = DateTime.Now.ToString() });
            int actual = controller.Count();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Remove_count_is_right()
        {
            var model = controller.Get(0);
            controller.Remove(model);

            var expected = modelsCount - 1;
            var actual = controller.Count();

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}

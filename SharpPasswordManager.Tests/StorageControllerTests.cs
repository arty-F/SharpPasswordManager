using NUnit.Framework;
using SharpPasswordManager.BL;
using System;
using System.Collections.Generic;

namespace SharpPasswordManager.Tests
{
    public class StorageControllerTests
    {
        int modelsCount = 3;
        StorageController<ModelMock> controller;

        [SetUp]
        public void Setup()
        {
            List<ModelMock> modelsList = new List<ModelMock>();
            modelsList.Clear();
            for (int i = 0; i < modelsCount; i++)
            {
                modelsList.Add(new ModelMock { String = i.ToString() });
            }
            controller = new StorageController<ModelMock>(modelsList);
        }

        [Test]
        public void Count_IsEqualModelsCount()
        {
            int expected = modelsCount;
            int result = controller.Count();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void PasteAt_ModelPastedAtIndex()
        {
            int index = modelsCount;
            DateTime expectedDate = new DateTime(2011, 11, 11);
            string expectedString = "string";
            ModelMock mockModel = new ModelMock { Date = new DateTime(2011, 11, 11), String = expectedString };

            controller.PasteAt(modelsCount, mockModel);
            ModelMock resultModel = controller.Get(index);

            Assert.Multiple(() =>
            {
                Assert.That(resultModel.Date, Is.EqualTo(expectedDate));
                Assert.That(resultModel.String, Is.EqualTo(expectedString));
            });
        }

        [Test]
        public void Get_GettingCorrectModel()
        {
            int expectedIndex = 0;
            int result = int.Parse(controller.Get(expectedIndex).String);

            Assert.That(result, Is.EqualTo(expectedIndex));
        }

        [Test]
        public void Add_ModelAdded()
        {
            controller.Add(new ModelMock());

            Assert.That(controller.Count(), Is.EqualTo(modelsCount + 1));
        }
    }
}

using NUnit.Framework;
using SharpPasswordManager.BL;
using System;
using System.Collections.Generic;
using System.Text;

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
                modelsList.Add(new ModelMock());
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
        public void PasteGet_ModelPastedAtIndex()
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
    }
}

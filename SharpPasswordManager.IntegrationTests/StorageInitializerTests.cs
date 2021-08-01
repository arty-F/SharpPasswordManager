using NUnit.Framework;
using SharpPasswordManager.BL.Security;
using SharpPasswordManager.BL.StorageLogic;
using SharpPasswordManager.DL.DataGenerators;
using SharpPasswordManager.IntegrationTests.Mocks;
using System.Linq;
using System.Threading.Tasks;

namespace SharpPasswordManager.IntegrationTests
{
    public class StorageInitializerTests
    {
        IStorageInitializer<ModelMock> initializer;

        IDataGenerator dataGenerator;

        ICryptographer cryptographer;


        [SetUp]
        public void Setup()
        {
            dataGenerator = new DataGenerator();

            string key = "1234567890asdfghjkl";
            cryptographer = new Cryptographer(key);

            initializer = new StorageInitializer<ModelMock>(dataGenerator, cryptographer);
        }

        [Test]
        public void GetData_returns_correct_data_count()
        {
            int expected = 10;

            var result = initializer.GetData(expected);
            int actual = result.Count();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetData_returns_correct_data_type()
        {
            var expectedType = typeof(ModelMock);

            var result = initializer.GetData(100);

            Assert.True(result.All(r => r.GetType() == expectedType));
        }

        [Test]
        public void GetData_has_no_empty_or_null()
        {
            var result = initializer.GetData(100);

            Assert.Multiple(() =>
            {
                Assert.True(result.All(r => !string.IsNullOrEmpty(r.Login)));
                Assert.True(result.All(r => !string.IsNullOrEmpty(r.Date)));
            });
        }

        [Test]
        public async Task GetDataAsync_returns_correct_data_count()
        {
            int expected = 10;

            var result = await initializer.GetDataAsync(expected);
            int actual = result.Count();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetDataAsync_returns_correct_data_type()
        {
            var expectedType = typeof(ModelMock);

            var result = await initializer.GetDataAsync(100);

            Assert.True(result.All(r => r.GetType() == expectedType));
        }

        [Test]
        public async Task GetDataAsync_has_no_empty_or_null()
        {
            var result = await initializer.GetDataAsync(100);

            Assert.Multiple(() =>
            {
                Assert.True(result.All(r => !string.IsNullOrEmpty(r.Login)));
                Assert.True(result.All(r => !string.IsNullOrEmpty(r.Date)));
            });
        }
    }
}

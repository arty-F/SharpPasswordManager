using NUnit.Framework;
using SharpPasswordManager.DL.DataGenerators;
using System;

namespace SharpPasswordManager.ModuleTests
{
    public class DataGeneratorTests
    {
        IDataGenerator dataGenerator;

        [SetUp]
        public void Setup()
        {
            dataGenerator = new DataGenerator();
        }

        [Test, Repeat(100)]
        public void GenerateRandomUrl_lenght_greater_than_zero()
        {
            var result = dataGenerator.GenerateRandomUrl();

            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test, Repeat(100)]
        public void GenerateRandomLogin_lenght_greater_than_zero()
        {
            var result = dataGenerator.GenerateRandomLogin();

            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test, Repeat(100)]
        public void GenerateRandomPassword_lenght_not_less_min_lenght()
        {
            var minLenght = 6;
            var result = dataGenerator.GenerateRandomPassword();

            Assert.That(result.Length, Is.GreaterThanOrEqualTo(minLenght));
        }

        [Test, Repeat(100)]
        public void GenerateRandomPassword_lenght_equal_to_parameter()
        {
            var lenght = 10;
            var result = dataGenerator.GenerateRandomPassword(lenght);

            Assert.That(result.Length, Is.EqualTo(lenght));
        }

        [Test, Repeat(100)]
        public void GenerateRandomDate_not_less_now()
        {
            var now = DateTime.Now;
            var result = Convert.ToDateTime(dataGenerator.GenerateRandomDate());
            
            Assert.That(result, Is.GreaterThanOrEqualTo(now));
        }
    }
}

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

        [Test, Repeat(1000)]
        public void GenerateRandomDescription_lenght_greater_than_zero()
        {
            string result = dataGenerator.GenerateRandomDescription();

            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test, Repeat(1000)]
        public void GenerateRandomLogin_lenght_greater_than_zero()
        {
            string result = dataGenerator.GenerateRandomLogin();

            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test, Repeat(1000)]
        public void GenerateRandomPassword_lenght_not_less_min_lenght()
        {
            int minLenght = 6;
            string result = dataGenerator.GenerateRandomPassword();

            Assert.That(result.Length, Is.GreaterThanOrEqualTo(minLenght));
        }

        [Test, Repeat(1000)]
        public void GenerateRandomPassword_lenght_equal_to_parameter()
        {
            int lenght = 10;
            string result = dataGenerator.GenerateRandomPassword(lenght);

            Assert.That(result.Length, Is.EqualTo(lenght));
        }

        [Test, Repeat(1000)]
        public void GenerateRandomDate_not_less_now()
        {
            DateTime now = DateTime.Now;
            DateTime result = Convert.ToDateTime(dataGenerator.GenerateRandomDate());
            
            Assert.That(result, Is.GreaterThanOrEqualTo(now));
        }
    }
}

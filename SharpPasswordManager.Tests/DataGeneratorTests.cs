using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using SharpPasswordManager.BL;

namespace SharpPasswordManager.Tests
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
        public void GenerateRandomDescription_LenghtGreaterThanZero()
        {
            string result = dataGenerator.GenerateRandomDescription();

            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test, Repeat(1000)]
        public void GenerateRandomLogin_LenghtGreaterThanZero()
        {
            string result = dataGenerator.GenerateRandomLogin();

            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test, Repeat(1000)]
        public void GenerateRandomPassword_LenghtNotLessMinLenght()
        {
            int minLenght = 6;
            string result = dataGenerator.GenerateRandomPassword();

            Assert.That(result.Length, Is.GreaterThanOrEqualTo(minLenght));
        }

        [Test, Repeat(1000)]
        public void GenerateRandomPassword_LenghtEqualToParameter()
        {
            int lenght = 10;
            string result = dataGenerator.GenerateRandomPassword(lenght);

            Assert.That(result.Length, Is.EqualTo(lenght));
        }

        [Test, Repeat(1000)]
        public void GenerateRandomDate_NotLessNow()
        {
            DateTime now = DateTime.Now;
            DateTime result = dataGenerator.GenerateRandomDate();

            Assert.That(result, Is.GreaterThanOrEqualTo(now));
        }
    }
}

using NUnit.Framework;
using SharpPasswordManager.BL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.Tests
{
    public class EncryptorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Encrypt_ResultIsNotSimilar()
        {
            IEncryptor encryptor = null;

            string password = "12345";
            string encryptedPassword = encryptor.Encypt(password);

            Assert.That(encryptedPassword, Is.Not.EqualTo(password));
        }

        [Test]
        public void Decrypt_ResultIsSimilar()
        {
            IEncryptor encryptor = null;

            string password = "12345";
            string encryptedPassword = encryptor.Encypt(password);
            string decryptedPassword = encryptor.Decrypt(encryptedPassword);

            Assert.That(password, Is.EqualTo(decryptedPassword));
        }
    }
}

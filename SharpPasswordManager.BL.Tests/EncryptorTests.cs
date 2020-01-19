using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.BL.Tests
{
    [TestFixture]
    public class EncryptorTests
    {
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

        [Test]
        public void ResultIsSimilar()
        {
            

            string password = "12345";
            string encryptedPassword = "12345";

            Assert.That(password, Is.EqualTo(encryptedPassword));
        }
    }
}

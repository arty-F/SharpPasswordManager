using NUnit.Framework;
using SharpPasswordManager.BL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPasswordManager.Tests
{
    public class CryptographerTests
    {
        Cryptographer cryptographer;

        [SetUp]
        public void Setup()
        {
            byte[] key = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F }; 
            byte[] iv = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            cryptographer = new Cryptographer(key, iv);
        }

        [Test]
        public void Encrypt_ResultIsNotSimilar()
        {
            string password = "12345";
            string encryptedPassword = cryptographer.Encypt(password);

            Assert.That(encryptedPassword, Is.Not.EqualTo(password));
        }

        [Test]
        public void Decrypt_ResultIsSimilar()
        {
            string password = "12345";
            string encryptedPassword = cryptographer.Encypt(password);
            string decryptedPassword = cryptographer.Decrypt(encryptedPassword);

            Assert.That(password, Is.EqualTo(decryptedPassword));
        }
    }
}

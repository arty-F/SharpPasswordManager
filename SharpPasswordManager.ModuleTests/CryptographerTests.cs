using NUnit.Framework;
using SharpPasswordManager.BL.Security;
using System;
using System.Security.Cryptography;

namespace SharpPasswordManager.ModuleTests
{
    public class CryptographerTests
    {
        ICryptographer cryptographer;

        [SetUp]
        public void Setup()
        {
            var key = "1234567890asdfghjkl";
            cryptographer = new Cryptographer(key);
        }

        [Test, Repeat(1000)]
        public void Encrypt_return_different_value()
        {
            var data = "12345abc";
            var actual = cryptographer.Encypt(data);

            Assert.AreNotEqual(data, actual);
        }

        [Test, Repeat(1000)]
        public void Encrypted_data_is_not_same()
        {
            var data = "12345abc";
            var firstEcrypted = cryptographer.Encypt(data);
            var secondEcrypted = cryptographer.Encypt(data);

            Assert.AreNotEqual(firstEcrypted, secondEcrypted);
        }

        [Test, Repeat(1000)]
        public void Decrypted_data_is_same()
        {
            var expected = "12345abc";
            var decrypted = cryptographer.Encypt(expected);
            var actual = cryptographer.Decrypt(decrypted);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ChangeKey_dont_broke_alghoritm()
        {
            var data = "12345abc";
            var newKey = "newKey";

            cryptographer.ChangeKey(newKey);
            var encrypted = cryptographer.Encypt(data);
            var actual = cryptographer.Decrypt(encrypted);

            Assert.AreEqual(data, actual);
        }

        [Test]
        public void ChangeKey_after_encrypt_throws_ex()
        {
            var data = "12345abc";
            var newKey = "newKey";

            var encrypted = cryptographer.Encypt(data);
            cryptographer.ChangeKey(newKey);

            Assert.Throws<CryptographicException>(() => cryptographer.Decrypt(encrypted));
        }

        [Test]
        public void Encrypt_thrown_ex_when_null_arg()
        {
            string data = null;

            Assert.Throws<ArgumentNullException>(() => cryptographer.Encypt(data));
        }

        [Test]
        public void Decrypt_thrown_ex_when_null_arg()
        {
            string data = null;

            Assert.Throws<ArgumentNullException>(() => cryptographer.Decrypt(data));
        }

        [Test]
        public void ChangeKey_thrown_ex_when_null_arg()
        {
            string data = null;

            Assert.Throws<ArgumentNullException>(() => cryptographer.ChangeKey(data));
        }
    }
}

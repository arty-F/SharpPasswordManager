using NUnit.Framework;
using SharpPasswordManager.BL;

namespace SharpPasswordManager.Tests
{
    public class CryptographerTests
    {
        Cryptographer cryptographer;

        [SetUp]
        public void Setup()
        {
            string key = "1234567890asdfghjkl";
            cryptographer = new Cryptographer(key);
        }

        [Test]
        public void Encrypt_ResultDifferentFromTheOriginal()
        {
            string password = "12345";
            string encryptedPassword = cryptographer.Encypt(password);

            Assert.That(encryptedPassword, Is.Not.EqualTo(password));
        }

        [Test]
        public void Decrypt_ResultIsSameAfterEncrypt()
        {
            string password = "12345";
            string encryptedPassword = cryptographer.Encypt(password);
            string decryptedPassword = cryptographer.Decrypt(encryptedPassword);

            Assert.That(password, Is.EqualTo(decryptedPassword));
        }

        [Test]
        public void Decrypt_EveryTimeTheCipherChanges()
        {
            string original = "Some string.";
            string result1 = cryptographer.Encypt(original);
            string result2 = cryptographer.Encypt(original);

            Assert.That(result1, Is.Not.EqualTo(result2));
        }
    }
}

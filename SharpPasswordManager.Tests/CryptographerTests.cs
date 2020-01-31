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
            byte[] key = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
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

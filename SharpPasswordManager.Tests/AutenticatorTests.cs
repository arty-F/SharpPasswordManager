using NUnit.Framework;
using SharpPasswordManager.BL;

namespace SharpPasswordManager.Tests
{
    public class AutenticatorTests
    {
        [Test]
        public void Encrypt_ResultDifferentFromTheOriginal()
        {
            string encryptedPassword = "12345";
            string password = "12345";
            Autenticator autenticator = new Autenticator();

            bool result = autenticator.Autenticate(password, encryptedPassword);
            bool expected = true;

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}

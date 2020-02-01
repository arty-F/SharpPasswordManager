using NUnit.Framework;
using SharpPasswordManager.BL;

namespace SharpPasswordManager.Tests
{
    public class AutenticatorTests
    {
        [Test]
        public void Encrypt_ResultDifferentFromTheOriginal()
        {
            string data = "12345";
            string password = "12345";
            Autenticator autenticator = new Autenticator(data);

            bool result = autenticator.Autenticate(password);
            bool expected = true;

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}

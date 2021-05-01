using NUnit.Framework;
using SharpPasswordManager.BL;
using System.Threading.Tasks;

namespace SharpPasswordManager.Tests
{
    public class AutenticatorTests
    {
        [Test]
        public async Task Autenticate_WithSamePassword()
        {
            string encryptedPassword = "12345";
            string password = "12345";
            Autenticator autenticator = new Autenticator();

            bool result = await autenticator.Autenticate(password, encryptedPassword);
            bool expected = true;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task Autenticate_WithDifferentPasswords()
        {
            string encryptedPassword = "12345";
            string password = "54321";
            Autenticator autenticator = new Autenticator();

            bool result = await autenticator.Autenticate(password, encryptedPassword);
            bool expected = false;

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}

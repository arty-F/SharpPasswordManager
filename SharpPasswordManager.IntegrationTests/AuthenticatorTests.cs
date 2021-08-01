using NUnit.Framework;
using SharpPasswordManager.BL.Security;
using System;
using System.Threading.Tasks;

namespace SharpPasswordManager.IntegrationTests
{
    public class AuthenticatorTests
    {
        IAuthenticator authenticator;

        ICryptographer cryptographer;

        string enteredPassword;

        string encryptedPassword;

        [SetUp]
        public void Setup()
        {
            enteredPassword = "12345";
            cryptographer = new Cryptographer(enteredPassword);
            authenticator = new Authenticator(cryptographer);
            encryptedPassword = cryptographer.Encypt(enteredPassword);
        }

        [Test]
        public async Task Authenticate_with_right_password_is_correct()
        {
            var actual = await authenticator.Authenticate(enteredPassword, encryptedPassword);

            Assert.IsTrue(actual);
        }

        [Test]
        public async Task Authenticate_with_one_char_wrong_password_is_incorrect()
        {
            var actual = await authenticator.Authenticate("12340", encryptedPassword);

            Assert.IsFalse(actual);
        }

        [Test]
        public async Task Authenticate_with_full_wrong_password_is_incorrect()
        {
            var actual = await authenticator.Authenticate("5432112345", encryptedPassword);

            Assert.IsFalse(actual);
        }

        [Test]
        public async Task Authenticate_with_empty_str_is_incorrect()
        {
            var actual = await authenticator.Authenticate(string.Empty, encryptedPassword);

            Assert.IsFalse(actual);
        }

        [Test]
        public void Authenticate_with_null_first_param_throw_ex()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await authenticator.Authenticate(null, encryptedPassword));
        }

        [Test]
        public void Authenticate_with_null_secont_param_throw_ex()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await authenticator.Authenticate(enteredPassword, null));
        }

        [Test]
        public void Authenticate_with_null_both_param_throw_ex()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await authenticator.Authenticate(null, null));
        }
    }
}

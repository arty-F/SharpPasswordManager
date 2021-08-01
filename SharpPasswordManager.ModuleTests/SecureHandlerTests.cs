using NUnit.Framework;
using SharpPasswordManager.BL.Handlers;
using System.Linq;

namespace SharpPasswordManager.ModuleTests
{
    public class SecureHandlerTests
    {
        ISecureHandler secureHandler;

        const string passwordKey = "Password";

        const string dataFileName = "Data.bin";

        const string categoriesFileName = "Categories.bin";

        [SetUp]
        public void Setup()
        {
            secureHandler = new SecureHandler();
        }

        [Test]
        public void SetSecretKey_is_setting_correct_value ()
        {
            var secretKey = "1abc2";
            secureHandler.SecretKey = secretKey;

            var actual = secureHandler.SecretKey;

            Assert.That(actual, Is.EqualTo(secretKey));
        }

        [Test]
        public void GetIndexOf_gets_right_index_with_numeric_key_and_zero_value()
        {
            int secretKey = 123;
            secureHandler.SecretKey = secretKey.ToString();

            int actual = secureHandler.GetIndexOf(0);

            Assert.That(actual, Is.EqualTo(secretKey));
        }

        [Test]
        public void GetIndexOf_gets_right_index_with_numeric_key_and_non_zero_value()
        {
            int secretKey = 123;
            secureHandler.SecretKey = secretKey.ToString();
            int value = 12345;
            int expected = value + secretKey;

            int actual = secureHandler.GetIndexOf(value);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetIndexOf_gets_right_index_with_string_key()
        {
            var secretKey = "Key";
            secureHandler.SecretKey = secretKey;

            var expectedKey = secretKey.Sum(c => c);
            var value = 123;
            var expected = expectedKey + value;

            var actual = secureHandler.GetIndexOf(value);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetIndexOf_gets_right_index_with_mixed_key()
        {
            var secretKey = "12Key3";
            secureHandler.SecretKey = secretKey;

            var expectedKey = secretKey.Sum(c => c);
            var value = 123;
            var expected = expectedKey + value;

            var actual = secureHandler.GetIndexOf(value);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void PasswordKey_is_not_changed()
        {
            var expected = passwordKey;

            var actual = secureHandler.PasswordKey;

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CategoriesFileName_is_not_changed()
        {
            var expected = categoriesFileName;

            var actual = secureHandler.CategoriesFileName;

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void DataFileName_is_not_changed()
        {
            var expected = dataFileName;

            var actual = secureHandler.DataFileName;

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}

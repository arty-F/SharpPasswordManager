﻿using NUnit.Framework;
using SharpPasswordManager.BL.Handlers;

namespace SharpPasswordManager.ModuleTests
{
    public class AppSettingsHandlerTests
    {
        IAppSettingsHandler settingsHandler;

        [SetUp]
        public void Setup()
        {
            settingsHandler = new AppSettingsHandler();
            settingsHandler.Clear();
        }

        [Test]
        public void AlreadyExist_is_false_with_cleared()
        {
            var key = "key";

            Assert.False(settingsHandler.AlreadyExist(key));
        }

        [Test]
        public void AlreadyExist_is_false_with_not_existed_key()
        {
            var key = "key";
            var value = "value";
            settingsHandler.Write(key, value);

            var notExistingKey = "str";

            Assert.False(settingsHandler.AlreadyExist(notExistingKey));
        }

        [Test]
        public void AlreadyExist_is_true_with_existed_key()
        {
            var key = "key";
            var value = "value";
            settingsHandler.Write(key, value);

            Assert.True(settingsHandler.AlreadyExist(key));
        }

        [Test]
        public void Write_is_writing_key()
        {
            var key = "key";
            var value = "value";
            settingsHandler.Write(key, value);

            Assert.True(settingsHandler.AlreadyExist(key));
        }

        [Test]
        public void GetByKey_returns_correct_value()
        {
            var key = "key";
            var expected = "value";
            settingsHandler.Write(key, expected);

            var actual = settingsHandler.GetByKey(key);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Write_existing_key_is_renew_value()
        {
            var key = "key";
            var value = "value";
            settingsHandler.Write(key, value);

            var expected = "newValue";
            settingsHandler.Write(key, expected);

            var actual = settingsHandler.GetByKey(key);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetByKey_returns_null_with_not_existed_key()
        {
            var key = "key";

            var actual = settingsHandler.GetByKey(key);

            Assert.IsNull(actual);
        }

        [Test]
        public void Clear_is_clearing_settings()
        {
            var key = "key";
            var value = "value";

            settingsHandler.Write(key, value);
            settingsHandler.Clear();

            Assert.False(settingsHandler.AlreadyExist(key));
        }

        [Test]
        public void Delete_is_deleting_key()
        {
            var key = "key";
            var value = "value";
            settingsHandler.Write(key, value);

            settingsHandler.Delete(key);

            Assert.False(settingsHandler.AlreadyExist(key));
        }

        [Test]
        public void Delete_is_not_thrown_ex_with_no_exist_key_deleting()
        {
            var key = "key";

            Assert.DoesNotThrow(() => settingsHandler.Delete(key));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPasswordManager.Handlers
{
    /// <summary>
    /// Used by manage app settings through <see cref="ConfigurationManager"/>.
    /// </summary>
    public class AppSettingsHandler : IAppSettingsHandler
    {
        /// <summary>
        /// Returns is there a key in app settings.
        /// </summary>
        /// <param name="key">Key of settings.</param>
        /// <returns></returns>
        public bool AlreadyExist(string key)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            if (settings[key] == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Get value of recieved key.
        /// </summary>
        /// <param name="key">Key of app setting.</param>
        /// <returns></returns>
        public string GetByKey(string key)
        {
            if (!AlreadyExist(key))
                return null;

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            return settings[key].Value;
        }

        /// <summary>
        /// Write value to key setting. If key doesn't exist, add that.
        /// </summary>
        /// <param name="key">Key of app setting.</param>
        /// <param name="value">Value of app setting.</param>
        public void Write(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            if (AlreadyExist(key))
                settings[key].Value = value;
            else
                settings.Add(key, value);

            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}

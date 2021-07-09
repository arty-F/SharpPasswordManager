using System.Configuration;

namespace SharpPasswordManager.Helpers
{
    /// <summary>
    /// Used by manage app settings through <see cref="ConfigurationManager"/>.
    /// </summary>
    public class AppSettingsHelper : IAppSettingsHelper
    {
        Configuration configFile;
        KeyValueConfigurationCollection settings;

        public AppSettingsHelper()
        {
            configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            settings = configFile.AppSettings.Settings;
        }

        #region Public methods

        public bool AlreadyExist(string key) => settings[key] != null;

        public string GetByKey(string key) => AlreadyExist(key) ? settings[key].Value : null;

        public void Write(string key, string value)
        {
            if (AlreadyExist(key))
                settings[key].Value = value;
            else
                settings.Add(key, value);

            RefreshConfig();
        }

        public void Delete(string key)
        {
            if (AlreadyExist(key))
                settings.Remove(key);

            RefreshConfig();
        }

        #endregion

        #region Private methods

        private void RefreshConfig()
        {
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }

        #endregion
    }
}

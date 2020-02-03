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
    public class AppSettingsHandler
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
    }
}

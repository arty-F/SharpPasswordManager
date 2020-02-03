using SharpPasswordManager.Handlers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class FirstLoadViewModel
    {
        const string key = "Password";
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public FirstLoadViewModel()
        {
            //string value = "12345";
            //var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //var settings = configFile.AppSettings.Settings;
            //if (settings[key] == null)
            //{
            //    settings.Add(key, value);
            //    MessageBox.Show("add");
            //}
            //else
            //{
            //    settings[key].Value = value;
            //    MessageBox.Show("edit");
            //}
            //configFile.Save(ConfigurationSaveMode.Modified);
            //ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);


            //var appSettings = ConfigurationManager.AppSettings;
            //var password = appSettings.AllKeys.Where(k => k == "Password").FirstOrDefault();
            //if (password == default)
            //{
            //    MessageBox.Show("bla");
            //}



            //foreach (var key in appSettings.AllKeys)
            //{
            //    MessageBox.Show($"Key: {key} Value: {appSettings[key]}");
            //}
        }

        private ICommand createPassword;
        public ICommand CreatePassword
        {
            get
            {
                return createPassword ?? (createPassword = new CommandHandler(TryWritePassword, () => true));
            }
        }
        private void TryWritePassword()
        {
            if (true)
            {
                //Password handler TryWrite()
                throw new NotImplementedException();
            }
        }
    }
}

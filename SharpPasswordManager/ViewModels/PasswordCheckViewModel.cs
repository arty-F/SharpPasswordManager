using SharpPasswordManager.Handlers;
using SharpPasswordManager.BL.Interfaces;
using System;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace SharpPasswordManager.ViewModels
{
    public class PasswordCheckViewModel
    {
        public string Password { get; set; }
        private readonly IAppSettingsHandler setting;
        private readonly IAuthenticator autenticator;

        public PasswordCheckViewModel(IAppSettingsHandler setting, IAuthenticator autenticator)
        {
            this.setting = setting;
            this.autenticator = autenticator;
        }

        private ICommand checkPasswordCmd;
        public ICommand CheckPasswordCmd
        {
            get
            {
                return checkPasswordCmd ?? (checkPasswordCmd = new CommandHandler(AccessCheckAsync, (object obj) => true));
            }
        }
        private async void AccessCheckAsync(object parameter)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            var passwordBox = parameter as PasswordBox;
            Password = passwordBox.Password;

            autenticator.ChangeKey(Password);
            bool isAutenticate = await autenticator.Autenticate(Password, setting.GetByKey(SecureManager.PasswordKey));

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;

            if (isAutenticate)
            {
                SecureManager.Key = Password;
                Views.MainView mainView = new Views.MainView();
                foreach (Window item in Application.Current.Windows)
                    if (item.DataContext == this)
                        item.Close();
                mainView.Show();
            }
            else
                MessageBox.Show("Wrong password.");
        }

        private ICommand closeCmd;
        public ICommand CloseCmd
        {
            get
            {
                return closeCmd ?? (closeCmd = new CommandHandler(Close, () => true));
            }
        }
        private void Close()
        {
            Application.Current.Shutdown();
        }
    }
}

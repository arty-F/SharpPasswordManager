using System.Windows.Input;
using System.Windows;
using SharpPasswordManager.BL.Handlers;
using SharpPasswordManager.BL.Security;
using SharpPasswordManager.Helpers;
using System.Windows.Controls;

namespace SharpPasswordManager.ViewModels
{
    public class PasswordCheckViewModel
    {
        public string Password { get; set; }
        private readonly IAppSettingsHelper setting;
        private readonly IAuthenticator autenticator;

        public PasswordCheckViewModel(IAppSettingsHelper setting, IAuthenticator autenticator)
        {
            this.setting = setting;
            this.autenticator = autenticator;
        }

        private ICommand checkPasswordCmd;
        public ICommand CheckPasswordCmd
        {
            get
            {
                return checkPasswordCmd ?? (checkPasswordCmd = new CommandHelper(AccessCheckAsync, (object obj) => true));
            }
        }
        private async void AccessCheckAsync(object parameter)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            var passwordBox = parameter as PasswordBox;
            Password = passwordBox.Password;

            autenticator.ChangeKey(Password);
            bool isAutenticate = await autenticator.Autenticate(Password, setting.GetByKey(SecureHandler.PasswordKey));

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;

            if (isAutenticate)
            {
                SecureHandler.Key = Password;
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
                return closeCmd ?? (closeCmd = new CommandHelper(Close, () => true));
            }
        }
        private void Close()
        {
            Application.Current.Shutdown();
        }
    }
}

using System.Windows.Input;
using System.Windows;
using SharpPasswordManager.BL.Handlers;
using SharpPasswordManager.BL.Security;
using SharpPasswordManager.Helpers;
using System.Windows.Controls;
using SharpPasswordManager.Infrastructure.Injector;

namespace SharpPasswordManager.ViewModels
{
    public class PasswordCheckViewModel
    {
        public string Password { get; set; }
        private readonly IAppSettingsHandler setting;
        private readonly IAuthenticator autenticator;
        private readonly ISecureHandler secureHandler;

        public PasswordCheckViewModel(Injector injector)
        {
            setting = injector.AppSettingsHandler;
            autenticator = injector.Authenticator;
            secureHandler = injector.SecureHandler;
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
            bool isAutenticate = await autenticator.Authenticate(Password, setting.GetByKey(secureHandler.PasswordKey));

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;

            if (isAutenticate)
            {
                secureHandler.SecretKey = Password;
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

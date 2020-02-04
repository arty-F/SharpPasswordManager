using SharpPasswordManager.BL.Interfaces;
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
        const string passwordKey = "Password";
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        private readonly IAppSettingsHandler setting;
        private readonly ICryptographer cryptographer;

        public FirstLoadViewModel(IAppSettingsHandler setting, ICryptographer cryptographer)
        {
            this.setting = setting;
            this.cryptographer = cryptographer;
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
            if (Password == ConfirmPassword)
            {
                string value = Password;
                if (cryptographer != null)
                {
                    cryptographer.ChangeKey(value);
                    value = cryptographer.Encypt(value);
                }
                setting.Write(passwordKey, value);
            }
            else
            {
                // Message box ?
                throw new NotImplementedException();
            }
        }
    }
}

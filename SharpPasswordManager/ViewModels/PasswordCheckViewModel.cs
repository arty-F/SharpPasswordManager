using SharpPasswordManager.Handlers;
using SharpPasswordManager.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class PasswordCheckViewModel
    {
        const string passwordKey = "Password";
        public string Password { get; set; }
        private readonly IAppSettingsHandler setting;
        private readonly IAuthenticator autenticator;

        public PasswordCheckViewModel(IAppSettingsHandler setting, IAuthenticator autenticator)
        {
            this.setting = setting;
            this.autenticator = autenticator;
        }


        private ICommand checkPassword;
        public ICommand CheckPassword
        {
            get
            {
                return checkPassword ?? (checkPassword = new CommandHandler(AccessCheck, () => true));
            }
        }
        private void AccessCheck()
        {
            if (Password == null)
                return;

            autenticator.ChangeKey(Password);
            if (autenticator.Autenticate(Password, setting.GetByKey(passwordKey)))
            {
                // access gained
                throw new NotImplementedException();
            }
            else
            {
                // access denied
                throw new NotImplementedException();
            }
        }
    }
}

using SharpPasswordManager.BL;
using SharpPasswordManager.BL.Interfaces;
using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Handlers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class FirstLoadViewModel
    {
        const string passwordKey = "Password";
        const string dataFileName = "Data.bin";
        const string categoriesFileName = "Categories.bin";

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

                Views.MainView mainView = new Views.MainView();
                foreach (Window item in Application.Current.Windows)
                    if (item.DataContext == this)
                        item.Close();

                Initialize();
                mainView.ShowDialog();
            }
            else
            {
                MessageBox.Show("Wrong password");
            }
        }

        // Create data and categories files
        private void Initialize()
        {
            string dataPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dataFileName);
            string categoriesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), categoriesFileName);

            var dataController = new StorageController<DataModel>(dataPath);
            var dataInitializer = new StorageInitializer<DataModel>(new DataGenerator(), new Cryptographer(setting.GetByKey(passwordKey)));
            dataController.CreateStorage(dataInitializer.GetData());

            var categoriesController = new StorageController<CategoryModel>(categoriesPath);
            categoriesController.CreateStorage();
        }

    }
}

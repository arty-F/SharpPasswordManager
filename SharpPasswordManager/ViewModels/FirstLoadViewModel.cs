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
using System.Text.RegularExpressions;
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
        const int minLength = 4;
        const int maxLenght = 8;

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        private readonly IAppSettingsHandler setting;
        private readonly ICryptographer cryptographer;

        public FirstLoadViewModel(IAppSettingsHandler setting, ICryptographer cryptographer)
        {
            this.setting = setting;
            this.cryptographer = cryptographer;
        }

        private ICommand createPasswordCmd;
        public ICommand CreatePasswordCmd
        {
            get
            {
                return createPasswordCmd ?? (createPasswordCmd = new CommandHandler(TryWritePassword, () => true));
            }
        }
        private void TryWritePassword()
        {
            if (Password == ConfirmPassword)
            {
                if (Password.Length >= minLength && Password.Length <= maxLenght)
                {
                    if (Regex.IsMatch(Password, @"^\d+$")) // Is digit only
                    {
                        SecureManager.Key = Password;
                        string value = Password;
                        if (cryptographer != null)
                        {
                            cryptographer.ChangeKey(SecureManager.Key);
                            value = cryptographer.Encypt(value);
                        }
                        setting.Write(passwordKey, value);

                        if (Initialize())
                        {
                            Views.MainView mainView = new Views.MainView();
                            foreach (Window item in Application.Current.Windows)
                                if (item.DataContext == this)
                                    item.Close();

                            mainView.ShowDialog();
                        }
                        else
                            setting.Delete(passwordKey);
                    }
                    else
                        MessageBox.Show("Password should consist only of digits.");
                }
                else
                    MessageBox.Show($"The number of password characters must be between {minLength} and {maxLenght}.");
            }
            else
                MessageBox.Show("Entered passwords are different.");
        }

        // Create data and categories files
        private bool Initialize()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var dataController = new StorageController<DataModel>(Path.Combine(assemblyPath, dataFileName));
            var dataInitializer = new StorageInitializer<DataModel>(new DataGenerator(), new Cryptographer(SecureManager.Key));
            try
            {
                dataController.CreateStorage(dataInitializer.GetData());
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Can't write data to {ex.Source}");
                return false;
            }

            var categoriesController = new StorageController<CategoryModel>(Path.Combine(assemblyPath, categoriesFileName));
            try
            {
                categoriesController.CreateStorage(new List<CategoryModel>());
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Can't write data to {ex.Source}");
                return false;
            }

            return true;
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

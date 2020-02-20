using SharpPasswordManager.BL;
using SharpPasswordManager.BL.Interfaces;
using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class FirstLoadViewModel : INotifyPropertyChanged
    {
        const string passwordKey = "Password";
        const string dataFileName = "Data.bin";
        const string categoriesFileName = "Categories.bin";
        const int minLength = 4;
        const int maxLenght = 9;    // Sure that password lenth gives a stock of values when using Int32 index type which return <SecureManager>!

        public Visibility LoadingPanelVisibility { get; set; } = Visibility.Hidden;
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        private readonly IAppSettingsHandler setting;
        private readonly ICryptographer cryptographer;

        private bool isUiEnabled = true;
        public bool IsUiEnabled
        {
            get { return isUiEnabled; }
            set 
            { 
                isUiEnabled = value;
                OnPropertyChanged(nameof(IsUiEnabled));
            }
        }


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
                return createPasswordCmd ?? (createPasswordCmd = new CommandHandler(TryWritePassword, UiIsEnabled));
            }
        }
        private async void TryWritePassword()
        {
            IsUiEnabled = false;
            if (Password == ConfirmPassword)
            {
                if (Password.Length >= minLength && Password.Length <= maxLenght)
                {
                    if (Regex.IsMatch(Password, @"^\d+$")) // Is digits only
                    {
                        SecureManager.Key = Password;
                        string value = Password;
                        if (cryptographer != null)
                        {
                            cryptographer.ChangeKey(SecureManager.Key);
                            value = cryptographer.Encypt(value);
                        }
                        setting.Write(passwordKey, value);

                        LoadingPanelVisibility = Visibility.Visible;
                        OnPropertyChanged(nameof(LoadingPanelVisibility));
                        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                        if (await Initialize())
                        {
                            Views.MainView mainView = new Views.MainView();
                            foreach (Window item in Application.Current.Windows)
                                if (item.DataContext == this)
                                    item.Close();

                            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                            mainView.ShowDialog();
                        }
                        else
                        {
                            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                            LoadingPanelVisibility = Visibility.Hidden;
                            OnPropertyChanged(nameof(LoadingPanelVisibility));
                            setting.Delete(passwordKey);
                        }
                    }
                    else
                        MessageBox.Show("Password should consist only of digits.");
                }
                else
                    MessageBox.Show($"The number of password characters must be between {minLength} and {maxLenght}.");
            }
            else
                MessageBox.Show("Entered passwords are different.");

            IsUiEnabled = true;
        }

        // Create data and category files
        private async Task<bool> Initialize()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var dataController = new StorageController<DataModel>(Path.Combine(assemblyPath, dataFileName));
            var dataInitializer = new StorageInitializer<DataModel>(new DataGenerator(), new Cryptographer(SecureManager.Key));
            try
            {
                List<DataModel> dataList = await dataInitializer.GetDataAsync();
                await dataController.CreateStorageAsync(dataList);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Can't write data to {ex.Source}");
                return false;
            }

            var categoriesController = new StorageController<CategoryModel>(Path.Combine(assemblyPath, categoriesFileName));
            try
            {
                await categoriesController.CreateStorageAsync(new List<CategoryModel>());
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
                return closeCmd ?? (closeCmd = new CommandHandler(Close, UiIsEnabled));
            }
        }
        private void Close()
        {
            Application.Current.Shutdown();
        }

        private bool UiIsEnabled()
        {
            return IsUiEnabled;
        }

        #region Property changing
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, args);
            }
        }
        #endregion
    }
}

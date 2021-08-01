using SharpPasswordManager.DL.DataGenerators;
using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class DataValidateViewModel
    {
        const int passwordMaxLength = 40;

        public DataModel Model { get; set; }
        private DataModel initialState = new DataModel();
        private PasswordBox passwordBox;

        public bool CanEditPassword { get; set; } = true;
        public Visibility GeneratePasswordVisibility { get; set; } = Visibility.Visible;
        public string GeneratePasswordCharacters { get; set; } = "14";
        IDataGenerator dataGenerator;

        public DataValidateViewModel(ref DataModel model, ref PasswordBox passwordBox, IDataGenerator dataGenerator = null)
        {
            Model = model;
            this.dataGenerator = dataGenerator;
            this.passwordBox = passwordBox;
            if (Model.Password != null)
            {
                passwordBox.Password = model.Password;
                CanEditPassword = false;
                GeneratePasswordVisibility = Visibility.Hidden;
            }
            if (Model.Date == null)
                Model.Date = DateTime.Now.ToString();

            initialState.Login = Model.Login;
            initialState.Url = Model.Url;
            initialState.Password = Model.Password;
        }

        private ICommand okCmd;
        public ICommand OkCmd
        {
            get
            {
                return okCmd ?? (okCmd = new CommandHelper(Validate, () => true));
            }
        }
        private void Validate()
        {
            Model.Password = passwordBox.Password;
            if (Model.Password != null && Model.Password.Length > 0)
            {
                if (Model.Url == null)
                    Model.Url = "";
                if (Model.Login == null)
                    Model.Login = "";

                CloseWindow();
            }
            else
                MessageBox.Show("Can't save data without password.");
        }

        private ICommand cancelCmd;
        public ICommand CancelCmd
        {
            get
            {
                return cancelCmd ?? (cancelCmd = new CommandHelper(RollBack, () => true));
            }
        }
        private void RollBack()
        {
            Model.Login = initialState.Login;
            Model.Url = initialState.Url;
            Model.Password = initialState.Password;

            CloseWindow();
        }

        private void CloseWindow()
        {
            foreach (Window item in Application.Current.Windows)
                if (item.DataContext == this)
                    item.Close();
        }

        private ICommand generatePasswordCmd;
        public ICommand GeneratePasswordCmd
        {
            get
            {
                return generatePasswordCmd ?? (generatePasswordCmd = new CommandHelper(GeneratePassword, () => true));
            }
        }
        private void GeneratePassword()
        {
            if (dataGenerator != null)
                if (int.TryParse(GeneratePasswordCharacters, out int passwordCharacters) && passwordCharacters > 0 && passwordCharacters < passwordMaxLength)
                    passwordBox.Password = dataGenerator.GenerateRandomPassword(passwordCharacters);
                else
                    MessageBox.Show($"Password lenth is must be a digit with characters count from 1 to {passwordMaxLength}.");
        }
    }
}
 
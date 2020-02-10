using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class DataValidateViewModel
    {
        public DataModel Model { get; set; }
        private DataModel initialState = new DataModel();

        public DataValidateViewModel(ref DataModel model)
        {
            Model = model;

            initialState.Login = Model.Login;
            initialState.Description = Model.Description;
            initialState.Password = Model.Password;
        }

        private ICommand okCmd;
        public ICommand OkCmd
        {
            get
            {
                return okCmd ?? (okCmd = new CommandHandler(Validate, () => true));
            }
        }
        private void Validate()
        {
            if (Model.Description != null && Model.Description.Length > 0)
            {
                CloseWindow();
            }
            else
                MessageBox.Show("Incorrect description.");
        }

        private ICommand cancelCmd;
        public ICommand CancelCmd
        {
            get
            {
                return cancelCmd ?? (cancelCmd = new CommandHandler(RollBack, () => true));
            }
        }
        private void RollBack()
        {
            Model.Login = initialState.Login;
            Model.Description = initialState.Description;
            Model.Password = initialState.Password;

            CloseWindow();
        }

        private void CloseWindow()
        {
            foreach (Window item in Application.Current.Windows)
                if (item.DataContext == this)
                    item.Close();
        }
    }
}

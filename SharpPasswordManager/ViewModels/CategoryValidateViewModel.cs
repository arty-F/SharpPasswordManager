using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Handlers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class CategoryValidateViewModel
    {
        public CategoryModel Model { get; set; }
        private CategoryModel initialState = new CategoryModel();

        public CategoryValidateViewModel(ref CategoryModel model)
        {
            Model = model;

            initialState.Name = Model.Name;
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
            if (Model.Name != null && Model.Name.Length > 0)
            {
                if (Model.DataIndexes == null)
                    Model.DataIndexes = new List<int>();

                CloseWindow();
            }
            else
                MessageBox.Show("Incorrect name.");
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
            Model.Name = initialState.Name;

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

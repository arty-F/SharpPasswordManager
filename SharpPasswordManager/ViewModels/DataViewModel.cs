using SharpPasswordManager.BL;
using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Handlers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class DataViewModel : INotifyPropertyChanged
    {
        public delegate CategoryModel DataAddHandler();
        public event DataAddHandler OnNeededCurrentCategory;

        public delegate void DataChangeHandler();
        public event DataChangeHandler OnDataChanged;

        public ObservableCollection<DataModel> DataList { get; set; } = new ObservableCollection<DataModel>();
        IStorageHandler<CategoryModel, DataModel> storageHandler;

        public DataModel SelectedData { get; set; }

        public DataViewModel(IStorageHandler<CategoryModel, DataModel> storageHandler)
        {
            this.storageHandler = storageHandler;
        }

        public void CategoryChanged(CategoryModel category)
        {
            GetData(category);
        }

        private void DataChanged()
        {
            OnDataChanged?.Invoke();
            CategoryModel currentCategory = OnNeededCurrentCategory?.Invoke();
            if (currentCategory == null)
                return;
            GetData(currentCategory);
        }

        private void GetData(CategoryModel category)
        {
            int index = -1;
            if (SelectedData != null)
                index = DataList.IndexOf(SelectedData);

            DataList.Clear();
            if (category != null)
            {
                foreach (var data in storageHandler.GetData(category))
                {
                    DataList.Add(data);
                }
            }
            OnPropertyChanged(nameof(DataList));

            if (index > -1 && index < DataList.Count)
                SelectedData = DataList[index];
            else
                SelectedData = null;
            OnPropertyChanged(nameof(SelectedData));
        }

        private ICommand addDataCmd;
        public ICommand AddDataCmd
        {
            get
            {
                return addDataCmd ?? (addDataCmd = new CommandHandler(AddData, () => true));
            }
        }
        private void AddData()
        {
            Views.DataValidateView validateView = new Views.DataValidateView();
            DataModel newData = new DataModel();
            DataValidateViewModel validateVM = new DataValidateViewModel(ref newData, ref validateView.passwordBox, new DataGenerator());
            validateView.DataContext = validateVM;
            validateView.ShowDialog();

            if (newData.Password != null)
            {
                CategoryModel currentCategory = OnNeededCurrentCategory?.Invoke();
                if (currentCategory == null)
                    return;

                try
                {
                    storageHandler.AddData(newData, currentCategory);
                }
                catch (FileNotFoundException ex)
                { MessageBox.Show($"File not found {ex.Message}."); }
                catch (InvalidOperationException ex)
                { MessageBox.Show($"Can't write data to file {ex.Message}."); }
                catch (Exception ex)
                { MessageBox.Show($"Something is wrong {ex.Message}."); }

                DataChanged();
            }
        }

        private ICommand editDataCmd;
        public ICommand EditDataCmd
        {
            get
            {
                return editDataCmd ?? (editDataCmd = new CommandHandler(EditData, () => true));
            }
        }
        private void EditData()
        {
            DataModel newData = new DataModel { Date = SelectedData.Date, Description = SelectedData.Description, Login = SelectedData.Login, Password = SelectedData.Password };
            Views.DataValidateView validateView = new Views.DataValidateView();
            DataValidateViewModel validateVM = new DataValidateViewModel(ref newData, ref validateView.passwordBox);
            validateView.DataContext = validateVM;
            validateView.ShowDialog();

            if (!SelectedData.Equals(newData))
            {
                try
                {
                    storageHandler.ReplaceData(SelectedData, newData);
                }
                catch (FileNotFoundException ex)
                { MessageBox.Show($"File not found {ex.Message}."); }
                catch (InvalidOperationException ex)
                { MessageBox.Show($"Can't write data to file {ex.Message}."); }
                catch (Exception ex)
                { MessageBox.Show($"Something is wrong {ex.Message}."); }

                DataChanged();
            }
        }

        private ICommand deleteDataCmd;
        public ICommand DeleteDataCmd
        {
            get
            {
                return deleteDataCmd ?? (deleteDataCmd = new CommandHandler(DeleteData, () => true));
            }
        }
        private void DeleteData()
        {
            СonfirmationViewModel confirmVM = new СonfirmationViewModel("Do you really want to delete this data?");
            Views.СonfirmationView confirmView = new Views.СonfirmationView();
            confirmView.DataContext = confirmVM;
            confirmView.ShowDialog();

            if (confirmVM.Result)
            {
                try
                {
                    storageHandler.RemoveData(SelectedData);
                }
                catch (FileNotFoundException ex)
                { MessageBox.Show($"File not found {ex.Message}."); }
                catch (InvalidOperationException ex)
                { MessageBox.Show($"Can't write data to file {ex.Message}."); }
                catch (Exception ex)
                { MessageBox.Show($"Something is wrong {ex.Message}."); }

                DataChanged();
            }
        }

        private ICommand copyDescriptionCmd;
        public ICommand CopyDescriptionCmd
        {
            get
            {
                return copyDescriptionCmd ?? (copyDescriptionCmd = new CommandHandler(CopyDescription, () => true));
            }
        }
        private void CopyDescription()
        {
            Clipboard.SetText(SelectedData.Description);
        }

        private ICommand copyLoginCmd;
        public ICommand CopyLoginCmd
        {
            get
            {
                return copyLoginCmd ?? (copyLoginCmd = new CommandHandler(CopyLogin, () => true));
            }
        }
        private void CopyLogin()
        {
            Clipboard.SetText(SelectedData.Login);
        }

        private ICommand copyPasswordCmd;
        public ICommand CopyPasswordCmd
        {
            get
            {
                return copyPasswordCmd ?? (copyPasswordCmd = new CommandHandler(CopyPassword, () => true));
            }
        }
        private void CopyPassword()
        {
            Clipboard.SetText(SelectedData.Password);
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

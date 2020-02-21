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
        public delegate void DataAddHandler();
        public event DataAddHandler OnDataChanged;

        public ObservableCollection<DataModel> DataList { get; set; }
        IStorageHandler<CategoryModel, DataModel> storageHandler;

        private DataModel selectedData;
        public DataModel SelectedData
        {
            get { return selectedData; }
            set
            {
                selectedData = value;
                OnPropertyChanged(nameof(SelectedData));
            }
        }

        public DataViewModel(IStorageHandler<CategoryModel, DataModel> storageHandler)
        {
            this.storageHandler = storageHandler;
        }

        public void CategoryChanged()
        {
            GetData();
        }

        private void GetData()
        {
            try
            {
                DataList = new ObservableCollection<DataModel>(storageHandler.GetData());
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"File not found {ex.Message}.");
                DataList = null;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Can't read data from file {ex.Message}.");
                DataList = null;
            }

            OnPropertyChanged(nameof(DataList));
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
            if (storageHandler.CurrentCategory == null)
                return;

            Views.DataValidateView validateView = new Views.DataValidateView();
            DataModel newData = new DataModel();
            DataValidateViewModel validateVM = new DataValidateViewModel(ref newData, ref validateView.passwordBox, new DataGenerator());
            validateView.DataContext = validateVM;
            validateView.ShowDialog();

            if (newData.Password != null)
            {
                try
                {
                    storageHandler.AddData(newData);
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show($"File not found {ex.Message}.");
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show($"Can't read data from file {ex.Message}.");
                }
                OnDataChanged?.Invoke();
                SelectedData = DataList[DataList.Count - 1];
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
            DataModel oldData = new DataModel();
            oldData.Date = selectedData.Date;
            oldData.Description = selectedData.Description;
            oldData.Login = selectedData.Login;
            oldData.Password = selectedData.Password;
            int index = DataList.IndexOf(selectedData);

            Views.DataValidateView validateView = new Views.DataValidateView();
            DataValidateViewModel validateVM = new DataValidateViewModel(ref selectedData, ref validateView.passwordBox);
            validateView.DataContext = validateVM;
            validateView.ShowDialog();

            if (!selectedData.Equals(oldData))
            {
                try
                {
                    storageHandler.ReplaceData(oldData, selectedData);
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show($"File not found {ex.Message}.");
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show($"Can't save data to file {ex.Message}.");
                }
            }
            OnDataChanged?.Invoke();
            SelectedData = DataList[index];
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
                storageHandler.RemoveData(selectedData);
                OnDataChanged?.Invoke();
                SelectedData = null;
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

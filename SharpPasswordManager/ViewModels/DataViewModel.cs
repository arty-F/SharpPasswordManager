using SharpPasswordManager.BL;
using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class DataViewModel : INotifyPropertyChanged
    {
        const string dataFileName = "Data.bin";
        private int startingIndex;
        private List<int> dataIndexes;

        public delegate void DataAddHandler(int index);
        public event DataAddHandler OnDataAdded;

        public delegate void DataDeleteHandler(int index);
        public event DataAddHandler OnDataDeleted;

        private StorageController<DataModel> dataController;
        public ObservableCollection<DataModel> DataList { get; set; }

        private DataModel selectedData;
        public DataModel SelectedData
        {
            get { return selectedData; }
            set { selectedData = value; }
        }

        public DataViewModel(int startingIndex)
        {
            this.startingIndex = startingIndex;
            dataController = new StorageController<DataModel>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dataFileName), new Cryptographer(SecureManager.Key));
        }

        public void CategoryChanged(List<int> indexes)
        {
            if (indexes == null)
            {
                DataList = null;
                OnPropertyChanged(nameof(DataList));
                return;
            }

            dataIndexes = indexes;
            GetData();
        }

        private void GetData()
        {
            DataList = new ObservableCollection<DataModel>();

            if (dataIndexes.Count > 0)
            {
                foreach (var index in dataIndexes)
                {
                    try
                    {
                        DataList.Add(dataController[SecureManager.GetIndexOf(index)]);
                    }
                    catch (FileNotFoundException ex)
                    {
                        MessageBox.Show($"File not found {ex.Message}.");
                        DataList = null;
                        break;
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show($"Can't read data from file {ex.Message}.");
                        DataList = null;
                        break;
                    }
                }
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
            if (DataList == null)
                return;

            Views.DataValidateView validateView = new Views.DataValidateView();
            DataModel newModel = new DataModel();
            DataValidateViewModel validateVM = new DataValidateViewModel(ref newModel, ref validateView.passwordBox, new DataGenerator());
            validateView.DataContext = validateVM;
            validateView.ShowDialog();

            if (newModel.Password != null)
            {
                try
                {
                    dataController.PasteAt(SecureManager.GetIndexOf(startingIndex), newModel);
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show($"File not found {ex.Message}.");
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show($"Can't read data from file {ex.Message}.");
                }
                OnDataAdded?.Invoke(startingIndex);
                ++startingIndex;
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
            Views.DataValidateView validateView = new Views.DataValidateView();
            DataValidateViewModel validateVM = new DataValidateViewModel(ref selectedData, ref validateView.passwordBox);
            validateView.DataContext = validateVM;
            validateView.ShowDialog();

            int index = DataList.IndexOf(selectedData);
            try
            {
                dataController.PasteAt(SecureManager.GetIndexOf(dataIndexes[index]), selectedData);
                GetData();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"File not found {ex.Message}.");
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Can't save data to file {ex.Message}.");
            }

            if (DataList[index] != null)
                selectedData = DataList[index];
            OnPropertyChanged(nameof(SelectedData));
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
            OnDataDeleted?.Invoke(dataIndexes[DataList.IndexOf(selectedData)]);
            GetData();
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

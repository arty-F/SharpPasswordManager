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

        public delegate List<int> DataAddHandler();
        public event DataAddHandler OnDataAdded;

        private StorageController<DataModel> dataController;
        public ObservableCollection<DataModel> DataList { get; set; }

        public DataModel SelectedData { get; set; }

        public DataViewModel()
        {
            dataController = new StorageController<DataModel>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dataFileName), new Cryptographer(SecureManager.Key));
        }

        public void CategoryChanged(List<int> indexes)
        {
            DataList = new ObservableCollection<DataModel>();

            if (indexes != null && indexes.Count > 0)
            {
                foreach (var index in indexes)
                {
                    try
                    {
                        DataList.Add(dataController[index]);
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
                    OnPropertyChanged(nameof(DataList));
                }
            }
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
            DataModel newModel = new DataModel();
            DataValidateViewModel validateVM = new DataValidateViewModel(ref newModel);
            Views.DataValidateView validateView = new Views.DataValidateView();
            validateView.DataContext = validateVM;
            validateView.ShowDialog();

            if (newModel.Password != null)
            {
                //TODO : ADD TO CONTROLLER
                DataList.Add(newModel);
                OnPropertyChanged(nameof(DataList));
            }

            //List<int> usingIndexes = OnDataAdded?.Invoke();

            //throw new NotImplementedException();

            //DataModel model = new DataModel { Description = "asd", Login = "Login", Password = "Password", Date = DateTime.Now };
            //dataController.PasteAt(0, model);
            //DataList.Add(model);
            //OnPropertyChanged(nameof(DataList));
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

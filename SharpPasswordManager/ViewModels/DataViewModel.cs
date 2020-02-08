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

        private StorageController<DataModel> dataController;
        public ObservableCollection<DataModel> DataList { get; set; }

        public DataViewModel()
        {
            dataController = new StorageController<DataModel>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dataFileName), new Cryptographer(SecureManager.Key));
        }

        public void CategoryChanged(List<int> indexes)
        {
           // MessageBox.Show(indexes.Count.ToString());
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

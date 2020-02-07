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
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        const string dataFileName = "Data.bin";
        const string categoriesFileName = "Categories.bin";

        public ObservableCollection<CategoryModel> CategoriesList { get; set; }
        public ObservableCollection<DataModel> DataList { get; set; }

        private StorageController<DataModel> dataController;
        private StorageController<CategoryModel> categoriesController;

        public MainViewModel()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            dataController = new StorageController<DataModel>(Path.Combine(assemblyPath, dataFileName), new Cryptographer(AppManager.Password));
            categoriesController = new StorageController<CategoryModel>(Path.Combine(assemblyPath, categoriesFileName));

            GetCategories();
        }

        private void GetCategories()
        {
            CategoriesList = new ObservableCollection<CategoryModel>(categoriesController.ToList());
            OnPropertyChanged(nameof(CategoriesList));
        }

        private ICommand addCategoryCmd;
        public ICommand AddCategoryCmd
        {
            get
            {
                return addCategoryCmd ?? (addCategoryCmd = new CommandHandler(AddCategory, () => true));
            }
        }
        private void AddCategory()
        {
            CategoryModel newCategory = new CategoryModel { DataIndexes = new List<int>(), Name = "Default"};
            try
            {
                categoriesController.Add(newCategory);
                GetCategories();
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

using SharpPasswordManager.BL;
using SharpPasswordManager.BL.Interfaces;
using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Handlers;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class MainViewModel
    {
        public UserControl CategoriesControl { get; set; }
        public UserControl DataControl { get; set; }

        public MainViewModel()
        {
            IStorageController<CategoryModel> categoryController = new StorageController<CategoryModel>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SecureManager.CategoriesFileName));
            IStorageController<DataModel> dataController = new StorageController<DataModel>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SecureManager.DataFileName), new Cryptographer(SecureManager.Key));

            IStorageHandler<CategoryModel, DataModel> storageHandler = new StorageHandler(categoryController, dataController);

            CategoriesControl = new Views.CategoryView();
            CategoryViewModel categoryVM = new CategoryViewModel(storageHandler);
            CategoriesControl.DataContext = categoryVM;

            DataControl = new Views.DataView();
            DataViewModel dataVM = new DataViewModel(storageHandler);
            DataControl.DataContext = dataVM;
            
            categoryVM.OnCategoryChanged += dataVM.CategoryChanged;
            dataVM.OnDataChanged += categoryVM.DataChanged;
        }

        private ICommand minimizeCmd;
        public ICommand MinimizeCmd
        {
            get
            {
                return minimizeCmd ?? (minimizeCmd = new CommandHandler(Minimize, () => true));
            }
        }
        private void Minimize()
        {
            foreach (Window item in Application.Current.Windows)
            {
                if (item.DataContext == this) item.WindowState = WindowState.Minimized;
            }
        }

        private ICommand closeCmd;
        public ICommand CloseCmd
        {
            get
            {
                return closeCmd ?? (closeCmd = new CommandHandler(Close, () => true));
            }
        }
        private void Close()
        {
            Application.Current.Shutdown();
        }
    }
}

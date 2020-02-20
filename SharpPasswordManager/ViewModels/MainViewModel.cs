using SharpPasswordManager.Handlers;
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
            CategoriesControl = new Views.CategoryView();
            CategoryViewModel categoryVM = new CategoryViewModel();
            CategoriesControl.DataContext = categoryVM;

            DataControl = new Views.DataView();
            DataViewModel dataVM = new DataViewModel(categoryVM.GetStartingIndex());
            DataControl.DataContext = dataVM;

            categoryVM.OnCategoryChanged += dataVM.CategoryChanged;
            dataVM.OnDataAdded += categoryVM.AddData;
            dataVM.OnDataDeleted += categoryVM.DeleteData;
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

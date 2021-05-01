﻿using SharpPasswordManager.BL;
using SharpPasswordManager.BL.Interfaces;
using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Handlers;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public UserControl CategoriesControl { get; set; }
        public UserControl DataControl { get; set; }

        public Visibility SecurePanelVisibility { get; set; } = Visibility.Hidden;
        IStorageHandler<CategoryModel, DataModel> storageHandler;

        public MainViewModel()
        {
            IStorageController<CategoryModel> categoryController = new StorageController<CategoryModel>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SecureManager.CategoriesFileName));
            IStorageController<DataModel> dataController = new StorageController<DataModel>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SecureManager.DataFileName), new Cryptographer(SecureManager.Key));

            storageHandler = new StorageHandler(categoryController, dataController);

            CategoriesControl = new Views.CategoryView();
            CategoryViewModel categoryVM = new CategoryViewModel(storageHandler);
            CategoriesControl.DataContext = categoryVM;

            DataControl = new Views.DataView();
            DataViewModel dataVM = new DataViewModel(storageHandler);
            DataControl.DataContext = dataVM;
            
            categoryVM.OnCategoryChanged += dataVM.CategoryChanged;
            dataVM.OnNeededCurrentCategory += categoryVM.ShareSelectedCategory;
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

        private ICommand secureCmd;
        public ICommand SecureCmd
        {
            get
            {
                return secureCmd ?? (secureCmd = new CommandHandler(Secure, () => true));
            }
        }
        private async void Secure()
        {
            SecurePanelVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(SecurePanelVisibility));
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            await storageHandler.SecureStorageAsync();
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
            SecurePanelVisibility = Visibility.Hidden;
            OnPropertyChanged(nameof(SecurePanelVisibility));
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

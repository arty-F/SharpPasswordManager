using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class CategoryViewModel : INotifyPropertyChanged
    {
        public delegate void CategoryChangeHandler();
        public event CategoryChangeHandler OnCategoryChanged;

        IStorageHandler<CategoryModel, DataModel> storageHandler;
        public ObservableCollection<CategoryModel> CategoriesList { get; set; }

        private CategoryModel selectedCategory;
        public CategoryModel SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                if (selectedCategory == null)
                    storageHandler.CurrentCategoryIndex = -1;
                else
                    storageHandler.CurrentCategoryIndex = CategoriesList.IndexOf(SelectedCategory);

                OnCategoryChanged?.Invoke();
            }
        }

        public CategoryViewModel(IStorageHandler<CategoryModel, DataModel> storageHandler)
        {
            this.storageHandler = storageHandler;
            GetCategories();
        }

        private void GetCategories()
        {
            try
            {
                CategoriesList = new ObservableCollection<CategoryModel>(storageHandler.GetCategories());
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"File not found {ex.Message}.");
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Can't read data from file {ex.Message}.");
            }

            OnPropertyChanged(nameof(CategoriesList));
        }

        public void DataChanged()
        {
            GetCategories();
            SelectedCategory = CategoriesList[storageHandler.CurrentCategoryIndex];
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
            CategoryModel newCategory = new CategoryModel();
            CategoryValidateViewModel validateVM = new CategoryValidateViewModel(ref newCategory);
            Views.CategoryValidateView validateView = new Views.CategoryValidateView();
            validateView.DataContext = validateVM;
            validateView.ShowDialog();

            if (newCategory.DataIndexes != null && newCategory.Name != null)
            {
                try
                {
                    storageHandler.AddCategory(newCategory);
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

                SelectedCategory = CategoriesList.LastOrDefault();
                OnCategoryChanged?.Invoke();
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        private ICommand deleteCategoryCmd;
        public ICommand DeleteCategoryCmd
        {
            get
            {
                return deleteCategoryCmd ?? (deleteCategoryCmd = new CommandHandler(DeleteCategory, () => true));
            }
        }
        private void DeleteCategory()
        {
            СonfirmationViewModel confirmVM = new СonfirmationViewModel("Do you really want to delete this category?");
            Views.СonfirmationView confirmView = new Views.СonfirmationView();
            confirmView.DataContext = confirmVM;
            confirmView.ShowDialog();

            if (confirmVM.Result)
            {
                try
                {
                    storageHandler.RemoveCategory(selectedCategory);
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

                SelectedCategory = null;
                OnCategoryChanged?.Invoke();
                OnPropertyChanged(nameof(CategoriesList));
            }
        }

        private ICommand editCategoryCmd;
        public ICommand EditCategoryCmd
        {
            get
            {
                return editCategoryCmd ?? (editCategoryCmd = new CommandHandler(EditCategory, () => true));
            }
        }
        private void EditCategory()
        {
            CategoryModel oldCategory = new CategoryModel();
            oldCategory.Name = selectedCategory.Name;
            oldCategory.DataIndexes = new List<int>(selectedCategory.DataIndexes);

            CategoryValidateViewModel validateVM = new CategoryValidateViewModel(ref selectedCategory);
            Views.CategoryValidateView validateView = new Views.CategoryValidateView();
            validateView.DataContext = validateVM;
            validateView.ShowDialog();

            if (!selectedCategory.Equals(oldCategory))
            {
                try
                {
                    storageHandler.ReplaceCategory(oldCategory, selectedCategory);
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

                SelectedCategory = CategoriesList[storageHandler.CurrentCategoryIndex];
                OnCategoryChanged?.Invoke();
                OnPropertyChanged(nameof(CategoriesList));
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

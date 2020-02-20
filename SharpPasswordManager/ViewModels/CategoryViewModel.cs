using SharpPasswordManager.BL;
using SharpPasswordManager.BL.Interfaces;
using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class CategoryViewModel : INotifyPropertyChanged
    {
        const string categoriesFileName = "Categories.bin";

        public delegate void CategoryChangeHandler(List<int> indexes);
        public event CategoryChangeHandler OnCategoryChanged;

        private IStorageController<CategoryModel> categoriesController;
        public ObservableCollection<CategoryModel> CategoriesList { get; set; }

        private CategoryModel selectedCategory;
        public CategoryModel SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                OnCategoryChanged?.Invoke(selectedCategory?.DataIndexes);
            }
        }

        public CategoryViewModel()
        {
            categoriesController = new StorageController<CategoryModel>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), categoriesFileName));
            GetCategories();
        }

        private void GetCategories()
        {
            try
            {
                CategoriesList = new ObservableCollection<CategoryModel>(categoriesController.ToList());
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

        public int GetStartingIndex()
        {
            int maxIndex = -1;

            foreach (var category in CategoriesList)
            {
                if (category.DataIndexes != null && category.DataIndexes.Count > 0)
                {
                    foreach (var index in category.DataIndexes)
                    {
                        if (index > maxIndex)
                            maxIndex = index;
                    }
                }
            }
            return ++maxIndex;
        }

        public void AddData(int dataIndex)
        {
            int selectedCategoryIndex = CategoriesList.IndexOf(selectedCategory);
            selectedCategory.DataIndexes.Add(dataIndex);
            try
            {
                categoriesController.PasteAt(selectedCategoryIndex, selectedCategory);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"File not found {ex.Message}.");
                selectedCategory.DataIndexes.RemoveAll(i => i == dataIndex);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Can't save data to file {ex.Message}.");
                selectedCategory.DataIndexes.RemoveAll(i => i == dataIndex);
            }
            GetCategories();
            SelectedCategory = CategoriesList[selectedCategoryIndex];
        }

        public void DeleteData(int dataIndex)
        {
            selectedCategory.DataIndexes.RemoveAll(i => i == dataIndex);

            int index = CategoriesList.IndexOf(selectedCategory);
            try
            {
                categoriesController.PasteAt(index, selectedCategory);
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

            if (CategoriesList[index] != null)
                selectedCategory = CategoriesList[index];
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
            CategoryModel newModel = new CategoryModel();
            CategoryValidateViewModel validateVM = new CategoryValidateViewModel(ref newModel);
            Views.CategoryValidateView validateView = new Views.CategoryValidateView();
            validateView.DataContext = validateVM;
            validateView.ShowDialog();

            if (newModel.DataIndexes != null && newModel.Name != null)
            {
                try
                {
                    categoriesController.Add(newModel);
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
                    categoriesController.Remove(selectedCategory);
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
                GetCategories();
                OnPropertyChanged(nameof(CategoriesList));
                OnCategoryChanged?.Invoke(null);
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
            CategoryValidateViewModel validateVM = new CategoryValidateViewModel(ref selectedCategory);
            Views.CategoryValidateView validateView = new Views.CategoryValidateView();
            validateView.DataContext = validateVM;
            validateView.ShowDialog();

            int index = CategoriesList.IndexOf(selectedCategory);
            try
            {
                categoriesController.PasteAt(index, selectedCategory);
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

            if (CategoriesList[index] != null)
                selectedCategory = CategoriesList[index];
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

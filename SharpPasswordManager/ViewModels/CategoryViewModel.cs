using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class CategoryViewModel : INotifyPropertyChanged
    {
        public delegate void CategoryChangeHandler(CategoryModel category);
        public event CategoryChangeHandler OnCategoryChanged;

        IStorageHandler<CategoryModel, DataModel> storageHandler;
        public ObservableCollection<CategoryModel> CategoriesList { get; set; } = new ObservableCollection<CategoryModel>();

        private CategoryModel selectedCategory;
        public CategoryModel SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                OnCategoryChanged?.Invoke(selectedCategory);
            }
        }

        public CategoryViewModel(IStorageHandler<CategoryModel, DataModel> storageHandler)
        {
            this.storageHandler = storageHandler;
            GetCategories();
        }

        private void GetCategories()
        {
            int index = -1;
            if (SelectedCategory != null)
                index = CategoriesList.IndexOf(SelectedCategory);

            CategoriesList.Clear();

            try
            {
                CategoriesList = new ObservableCollection<CategoryModel>(storageHandler.GetCategories());
            }
            catch (FileNotFoundException ex)
            { MessageBox.Show($"File not found {ex.Message}."); }
            catch (InvalidOperationException ex)
            { MessageBox.Show($"Can't read data from file {ex.Message}."); }
            catch (Exception ex)
            { MessageBox.Show($"Something is wrong {ex.Message}."); }

            OnPropertyChanged(nameof(CategoriesList));

            if (index > -1 && index < CategoriesList.Count)
                SelectedCategory = CategoriesList[index];
            else
                SelectedCategory = null;

            OnPropertyChanged(nameof(SelectedCategory));
        }

        public CategoryModel ShareSelectedCategory()
        {
            return SelectedCategory;
        }

        public void DataChanged()
        {
            GetCategories();
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
                }
                catch (FileNotFoundException ex)
                { MessageBox.Show($"File not found {ex.Message}."); }
                catch (InvalidOperationException ex)
                { MessageBox.Show($"Can't write data to file {ex.Message}."); }
                catch (Exception ex)
                { MessageBox.Show($"Something is wrong {ex.Message}."); }

                GetCategories();
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
                    storageHandler.RemoveCategory(SelectedCategory);
                }
                catch (FileNotFoundException ex)
                { MessageBox.Show($"File not found {ex.Message}."); }
                catch (InvalidOperationException ex)
                { MessageBox.Show($"Can't save data to file {ex.Message}."); }
                catch (Exception ex)
                { MessageBox.Show($"Something is wrong {ex.Message}."); }

                SelectedCategory = null;
                GetCategories();
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
            CategoryModel newCategory = new CategoryModel { DataIndexes = new List<int>(SelectedCategory?.DataIndexes), Name = SelectedCategory.Name };
            CategoryValidateViewModel validateVM = new CategoryValidateViewModel(ref newCategory);
            Views.CategoryValidateView validateView = new Views.CategoryValidateView();
            validateView.DataContext = validateVM;
            validateView.ShowDialog();

            if (!SelectedCategory.Equals(newCategory))
            {
                try
                {
                    storageHandler.ReplaceCategory(SelectedCategory, newCategory);
                }
                catch (FileNotFoundException ex)
                { MessageBox.Show($"File not found {ex.Message}."); }
                catch (InvalidOperationException ex)
                { MessageBox.Show($"Can't save data to file {ex.Message}."); }
                catch (Exception ex)
                { MessageBox.Show($"Something is wrong {ex.Message}."); }
                GetCategories();
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

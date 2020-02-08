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
            DataViewModel dataVM = new DataViewModel();
            DataControl.DataContext = dataVM;

            categoryVM.OnCategoryChanged += dataVM.CategoryChanged;
        }
    }
}

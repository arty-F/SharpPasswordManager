using SharpPasswordManager.BL;
using SharpPasswordManager.DL.Models;
using SharpPasswordManager.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpPasswordManager.ViewModels
{
    public class MainViewModel
    {
        const string dataFileName = "Data.bin";
        const string categoriesFileName = "Categories.bin";

        public List<CategoryModel> CategoriesList { get; set; }
        public List<DataModel> DataList { get; set; }

        private StorageController<DataModel> dataController;
        private StorageController<CategoryModel> categoriesController;

        public MainViewModel()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            dataController = new StorageController<DataModel>(Path.Combine(assemblyPath, dataFileName), new Cryptographer(AppManager.Password));
            categoriesController = new StorageController<CategoryModel>(Path.Combine(assemblyPath, categoriesFileName));

            CategoriesList = categoriesController.ToList();
        }


    }
}

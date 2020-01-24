using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using SharpPasswordManager.DL.Models;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharpPasswordManager.BL
{
    //TODO REWORK TO GENERIC
    public class StorageController //: IStorageController
    {
        private readonly string dataPath;
        private readonly string categoriesPath;
        private readonly IEncryptor encryptor;

        private List<DataModel> dataList { get; set; } = null;
        private List<CategoryModel> categoriesList { get; set; } = null;

        public StorageController(string dataPath, string categoriesPath, IEncryptor encryptor = null)
        {
            this.dataPath = dataPath;
            this.categoriesPath = categoriesPath;
            this.encryptor = encryptor;
        }


        /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
         * Return password value at the index parameter. If <dataList> not initialize, then call <SetDataList>
         method.
        !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
        public string GetData(int index)
        {
            CheckDataList();
            if (encryptor != null)
            {
                return encryptor.Decrypt(dataList[index].Password);
            }
            else
            {
                return dataList[index].Password;
            }
        }

        /*----------------------------------------------------------------------------------------------------
         * Make sure that dataList is created.
        ----------------------------------------------------------------------------------------------------*/
        private void CheckDataList()
        {
            if (dataList == null)
            {
                SetDataList();
            }
        }

        /*----------------------------------------------------------------------------------------------------
         * Deserialize <dataPath> file in to the List<DataModel>.
        ----------------------------------------------------------------------------------------------------*/
        private void SetDataList()
        {
            if (!File.Exists(dataPath))
            {
                throw new FileNotFoundException($"{dataPath} not found.");
            }
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                dataList = (List<DataModel>)formatter.Deserialize(stream);
            }
            catch (Exception)
            {
                throw new InvalidCastException($"Impossible deserialize {dataPath} file. Data was corrupted.");
            }
            finally
            {
                stream.Close();
            }
        }


        /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
         * Creating <DateModel> with values getting by parameters
        !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
        public void PasteData(int index, int categoryId, string description, string login, DateTime date, string password)
        {
            CheckDataList();
            int i = GetRealIndex(index);

            DataModel model = new DataModel();
            model.Date = date;
            if (encryptor != null)
            {
                model.Description = encryptor.Encypt(description);
                model.Login = encryptor.Encypt(login);
                model.Password = encryptor.Encypt(password);
            }
            else
            {
                model.Description = description;
                model.Login = login;
                model.Password = password;
            }
            dataList[i] = model;
            //ADD THIS DATA TO CATEGORY
            throw new NotImplementedException();
            //WriteData();
        }

        /*----------------------------------------------------------------------------------------------------
         * Setting index in of <dataList.Count> range.
        ----------------------------------------------------------------------------------------------------*/
        private int GetRealIndex(int index)
        {
            int i = index;
            while (i >= dataList.Count)
            {
                i -= dataList.Count;
            }
            return i;
        }

        public void DeleteData(int index)
        {
            throw new NotImplementedException();
        }

        public object GetCategories()
        {
            throw new NotImplementedException();
        }

        public void PasteCategory()
        {
            throw new NotImplementedException();
        }

        public void DeleteCategory(int id)
        {
            throw new NotImplementedException();
        }

        public void RenameCategory(int id, string name)
        {
            throw new NotImplementedException();
        }
    }
}

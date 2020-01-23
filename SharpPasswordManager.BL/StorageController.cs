using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using SharpPasswordManager.DL.Models;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharpPasswordManager.BL
{
    public class StorageController<TModel> : IStorageController<TModel>
    {
        private readonly string dataPath;
        private readonly string categoriesPath;
        private readonly IEncryptor encryptor = null;
        private readonly IDataGenerator dataGenerator = null;
        private readonly int modelsCount;

        private List<DataModel> dataList { get; set; } = null;
        private List<CategoryModel> categoriesList { get; set; } = null;

        public StorageController(string dataPath, string categoriesPath, IEncryptor encryptor = null, IDataGenerator dataGenerator = null, int modelsCount = 100000)
        {
            this.dataPath = dataPath;
            this.categoriesPath = categoriesPath;
            if (encryptor != null)
            {
                this.encryptor = encryptor;
            }
            if (dataGenerator != null)
            {
                this.dataGenerator = dataGenerator;
            }
            this.modelsCount = modelsCount;
        }


        /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
         * Сreating files that contain serialized data and categories.
        !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
        public void Initialize()
        {
            if (dataGenerator == null)
            {
                throw new ArgumentException("Data generator not exist.");
            }

            if (File.Exists(dataPath))
            {
                List<DataModel> dataList = CreateNewData(modelsCount);
                WriteData();
            }
            else
            {
                throw new FieldAccessException($"{dataPath} file is already created.");
            }

            if (File.Exists(categoriesPath))
            {
                Stream stream = new FileStream(categoriesPath, FileMode.Create, FileAccess.Write, FileShare.None);
                stream.Close();
            }
            else
            {
                throw new FieldAccessException($"{categoriesPath} file is already created.");
            }

        }

        /*----------------------------------------------------------------------------------------------------
         * Create list of <DataModel> and generate each field of DataModel with help of <dataGenerator>, if
         <encryptor> is not null then generated data will be encrypted.
        ----------------------------------------------------------------------------------------------------*/
        private List<DataModel> CreateNewData(int modelsCount)
        {
            if (dataGenerator == null)
            {
                return null;
            }

            List<DataModel> dataList = new List<DataModel>();

            // With encryption
            if (encryptor != null)
            {
                for (int i = 0; i < modelsCount; i++)
                {
                    DataModel data = new DataModel
                    {
                        Date = dataGenerator.GenerateRandomDate(),
                        Description = encryptor.Encypt(dataGenerator.GenerateRandomDescription()),
                        Login = encryptor.Encypt(dataGenerator.GenerateRandomLogin()),
                        Password = encryptor.Encypt(dataGenerator.GenerateRandomPassword())
                    };
                    dataList.Add(data);
                }
            }
            // Without encryption
            else
            {
                for (int i = 0; i < modelsCount; i++)
                {
                    DataModel data = new DataModel
                    {
                        Date = dataGenerator.GenerateRandomDate(),
                        Description = dataGenerator.GenerateRandomDescription(),
                        Login = dataGenerator.GenerateRandomLogin(),
                        Password = dataGenerator.GenerateRandomPassword()
                    };
                    dataList.Add(data);
                }
            }

            return dataList;
        }

        /*----------------------------------------------------------------------------------------------------
         * Serealize <dataList> to <dataPath> file.
        ----------------------------------------------------------------------------------------------------*/
        private void WriteData()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(dataPath, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, dataList);
            stream.Close();
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
            WriteData();
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

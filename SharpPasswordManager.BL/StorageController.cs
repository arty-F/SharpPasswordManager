using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using SharpPasswordManager.DL.Models;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharpPasswordManager.BL
{
    public class StorageController : IStorageController
    {
        private readonly string dataPath;
        private readonly string categoriesPath;
        private readonly IEncryptor encryptor = null;
        private readonly IDataGenerator dataGenerator = null;
        private readonly int modelsCount;

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

        /*----------------------------------------------------------------------------------------------------
         * Сreating files that contain serialized data and categories.
        ----------------------------------------------------------------------------------------------------*/
        public void Initialize()
        {
            if (dataGenerator == null)
            {
                throw new ArgumentException("Data generator not exist.");
            }

            if (File.Exists(dataPath))
            {
                List<DataModel> dataList = CreateData(modelsCount);
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(dataPath, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, dataList);
                stream.Close();
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
        private List<DataModel> CreateData(int modelsCount)
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
                        Id = i,
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
                        Id = i,
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




        public string GetData(int index)
        {
            throw new NotImplementedException();
        }

        public void PasteData(int index, int categoryId)
        {
            throw new NotImplementedException();
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

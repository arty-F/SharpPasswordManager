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
        private readonly string path;
        private readonly ICryptographer cryptographer;

        private List<TModel> modelList { get; set; } = null;

        public StorageController(string path, ICryptographer cryptographer = null)
        {
            this.path = path;
            this.cryptographer = cryptographer;
        }


        /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
         * Return <TModel> instance by received index.
        !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
        public TModel Get(int index)
        {
            CheckModelList();
            if (cryptographer == null)
            {
                return modelList[ReceiveIndexInRange(index)];
            }
            else
            {
                return ModelAfterCryptography(modelList[ReceiveIndexInRange(index)], CryptographyMode.Decrypt);
            }
        }


        /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
         * Paste <TModel> by received index in to <dataList> collection and serealize to file.
        !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
        public void PasteAt(int index, TModel model)
        {
            if (cryptographer == null)
            {
                modelList[ReceiveIndexInRange(index)] = model;
            }
            else
            {
                modelList[ReceiveIndexInRange(index)] = ModelAfterCryptography(model, CryptographyMode.Encrypt);
            }
            SaveChanges();
        }


        /*----------------------------------------------------------------------------------------------------
         * Make sure the <modelList> is initialized. If not call <ReceiveModels()>.
        ----------------------------------------------------------------------------------------------------*/
        private void CheckModelList()
        {
            if (modelList == null)
            {
                ReceiveModels();
            }
        }

        /*----------------------------------------------------------------------------------------------------
         * Deserealize file from <path> using binary formatter in to List<TModel>.
        ----------------------------------------------------------------------------------------------------*/
        private List<TModel> ReceiveModels()
        {
            var list = new List<TModel>();

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path} not found.");
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                modelList = (List<TModel>)formatter.Deserialize(stream);
            }
            catch (Exception)
            {
                throw new InvalidCastException($"Impossible deserialize {path} file. Data was corrupted.");
            }
            finally
            {
                stream.Close();
            }
            return list;
        }

        /*----------------------------------------------------------------------------------------------------
         * Decrease index until it remains within the range (not more or equal <modelList.Count> value).
        ----------------------------------------------------------------------------------------------------*/
        private int ReceiveIndexInRange(int index)
        {
            int i = index;
            while (i >= modelList.Count)
            {
                i -= modelList.Count;
            }
            return i;
        }

        /*----------------------------------------------------------------------------------------------------
         * Return a copy of <TModel> by received index with encrypted/decrypted all string properties.
        ----------------------------------------------------------------------------------------------------*/
        private TModel ModelAfterCryptography(TModel model, CryptographyMode mode)
        {
            dynamic cryptedModel = Activator.CreateInstance(typeof(TModel));

            foreach (var prop in model.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    string propertyValue = prop.GetValue(model).ToString();
                    if (mode == CryptographyMode.Decrypt)
                    {
                        prop.SetValue(cryptedModel, cryptographer.Decrypt(propertyValue));
                    }
                    else if (mode == CryptographyMode.Encrypt)
                    {
                        prop.SetValue(cryptedModel, cryptographer.Encypt(propertyValue));
                    }
                }
            }
            return cryptedModel;
        }

        /*----------------------------------------------------------------------------------------------------
         * Serealize <modelList> to <path> file by binary formatter.
        ----------------------------------------------------------------------------------------------------*/
        private void SaveChanges()
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path} not found.");
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.Read);
            try
            {
                formatter.Serialize(stream, modelList);
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"Impossible serealize data to {path} file.");
            }
            finally
            {
                stream.Close();
            }
        }
    }
}
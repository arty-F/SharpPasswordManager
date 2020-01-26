using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharpPasswordManager.BL
{
    /// <summary>
    /// Manage models collection taken from file. Can get, paste and find count of models.
    /// </summary>
    /// <typeparam name="TModel">Any model class.</typeparam>
    public class StorageController<TModel> : IStorageController<TModel>
    {
        private readonly string path;
        private readonly ICryptographer cryptographer;

        private List<TModel> modelList { get; set; } = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageController{TModel}"/>.
        /// </summary>
        /// <param name="path">Path to data file.</param>
        /// <param name="cryptographer">Using for data encryption/decryption.</param>
        public StorageController(string path, ICryptographer cryptographer = null)
        {
            this.path = path;
            this.cryptographer = cryptographer;
        }

        /// <summary>
        /// Return <see cref="TModel"/> instance by received index.
        /// </summary>
        /// <param name="index">Element index.</param>
        /// <returns>Model at received index.</returns>
        public TModel Get(int index)
        {
            CheckModelList();

            if (cryptographer == null)
                return modelList[ReceiveIndexInRange(index)];
            else
                return ModelAfterCryptography(modelList[ReceiveIndexInRange(index)], CryptographyMode.Decrypt);

        }

        /// <summary>
        /// Paste <see cref="TModel"/> by received index in to controller collection and serealize to file.
        /// </summary>
        /// <param name="index">Index to insert.</param>
        /// <param name="model">Model to insert.</param>
        public void PasteAt(int index, TModel model)
        {
            CheckModelList();

            if (cryptographer == null)
                modelList[ReceiveIndexInRange(index)] = model;
            else
                modelList[ReceiveIndexInRange(index)] = ModelAfterCryptography(model, CryptographyMode.Encrypt);

            SaveChanges();
        }

        /// <summary>
        /// Return count of models collection.
        /// </summary>
        /// <returns>Count of models collection.</returns>
        public int Count()
        {
            CheckModelList();

            return modelList.Count;
        }

        /*----------------------------------------------------------------------------------------------------
         * Make sure the <modelList> is initialized. If not call <ReceiveModels()>.
        ----------------------------------------------------------------------------------------------------*/
        private void CheckModelList()
        {
            if (modelList == null)
                ReceiveModels();
        }

        /*----------------------------------------------------------------------------------------------------
         * Deserealize file from <path> using binary formatter in to List<TModel>.
        ----------------------------------------------------------------------------------------------------*/
        private List<TModel> ReceiveModels()
        {
            var list = new List<TModel>();

            if (!File.Exists(path))
                throw new FileNotFoundException($"{path} not found.");

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
                i -= modelList.Count;

            return i;
        }

        /*----------------------------------------------------------------------------------------------------
         * Return a copy of <TModel> with encrypted/decrypted string properties.
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
                        prop.SetValue(cryptedModel, cryptographer.Decrypt(propertyValue));
                    else if (mode == CryptographyMode.Encrypt)
                        prop.SetValue(cryptedModel, cryptographer.Encypt(propertyValue));
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
                throw new FileNotFoundException($"{path} not found.");

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
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SharpPasswordManager.BL.Enums;
using SharpPasswordManager.BL.Interfaces;

namespace SharpPasswordManager.BL
{
    /*----------------------------------------------------------------------------------
     * Exceptions:

            FileNotFoundException - When handling all of <IStorageController> methods,
                                    occurs when <modelList> not initialized and file
                                    by <path> not found. May also occur when using
                                    <PasteAt> method if the file cannot be found.

            InvalidOperationException - When it is impossible to serealize/deserealize 
                                        model list to a file by <path>.
    ----------------------------------------------------------------------------------*/
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
        /// Initializes a new instance of the <see cref="StorageController{TModel}"/>.
        /// </summary>
        /// <param name="modelList">Collection of models.</param>
        /// <param name="cryptographer">Using for data encryption/decryption.</param>
        public StorageController(List<TModel> modelList, ICryptographer cryptographer = null)
        {
            this.modelList = modelList;
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
                return ApplyCryptography(modelList[ReceiveIndexInRange(index)], CryptographyMode.Decrypt);
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
                modelList[ReceiveIndexInRange(index)] = ApplyCryptography(model, CryptographyMode.Encrypt);

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

        /// <summary>
        /// Create file by path inner value directory and write modelsList into him.
        /// </summary>
        public void CreateStorage(IEnumerable<TModel> models = null)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            try
            {
                if (models != null)
                    formatter.Serialize(stream, models);
            }
            catch (Exception)
            {
                throw new InvalidOperationException(path);
            }
            finally
            {
                stream.Close();
            }
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
        private void ReceiveModels()
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                modelList = (List<TModel>)formatter.Deserialize(stream);
            }
            catch (Exception)
            {
                throw new InvalidOperationException(path);
            }
            finally
            {
                stream.Close();
            }
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
        private TModel ApplyCryptography(TModel model, CryptographyMode mode)
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
            if (path == null)
                return;

            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.Read);
            try
            {
                formatter.Serialize(stream, modelList);
            }
            catch (Exception)
            {
                throw new InvalidOperationException(path);
            }
            finally
            {
                stream.Close();
            }
        }
    }
}
﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SharpPasswordManager.BL.Enums;
using System.Threading.Tasks;
using SharpPasswordManager.BL.Security;
using System.Text.Json;
using System.Linq;
using System.Text;

namespace SharpPasswordManager.BL.StorageLogic
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
    /// Manage models collection taken from file. Can get, add, remove, paste and find count of models.
    /// </summary>
    /// <typeparam name="TModel">Any model class.</typeparam>
    public class StorageController<TModel> : IStorageController<TModel>
    {
        private readonly string path;
        private readonly ICryptographer cryptographer;

        private List<TModel> modelList { get; set; } = null;

        public TModel this[int i]
        {
            get { return Get(i); }
            set { PasteAt(i, value); }
        }

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
            this.modelList = new List<TModel>(modelList.Count);
            modelList.ForEach(m => Add(m));

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
        /// Add <see cref="TModel"/> in to controller collection and serealize to file.
        /// </summary>
        /// <param name="model">Model to adding/</param>
        public void Add(TModel model)
        {
            CheckModelList();

            if (cryptographer == null)
                modelList.Add(model);
            else
                modelList.Add(ApplyCryptography(model, CryptographyMode.Encrypt));

            SaveChanges();
        }

        /// <summary>
        /// Remove all same models from the collection.
        /// </summary>
        /// <param name="model">Model to remove.</param>
        public void Remove(TModel model)
        {
            CheckModelList();

            if (cryptographer != null)
            {
                foreach (var m in modelList)
                {
                    if (model.Equals(ApplyCryptography(m, CryptographyMode.Decrypt)))
                    {
                        modelList.Remove(m);
                        break;
                    }
                }
            }
            else
                modelList.RemoveAll(m => m.Equals(model));

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
        /// Return all models in storage.
        /// </summary>
        /// <returns></returns>
        public List<TModel> ToList()
        {
            CheckModelList();

            if (cryptographer == null)
                return modelList;
            else
                return ApplyCryptography(modelList, CryptographyMode.Decrypt);
        }

        /// <summary>
        /// Asynchronously create file by path inner value directory and write modelsList into him.
        /// </summary>
        /// <param name="models">Writed to file models.</param>
        /// <returns></returns>
        public async Task CreateStorageAsync(IEnumerable<TModel> models = null)
        {
            await Task.Run(() => CreateStorage(models));
        }

        /// <summary>
        /// Create file by path inner value directory and write modelsList into him.
        /// </summary>
        /// <param name="models">Writed to file models.</param>
        public void CreateStorage(IEnumerable<TModel> models = null)
        {
            if (models == null)
                return;

            using var writer = new BinaryWriter(File.Open(path, FileMode.Create), Encoding.UTF8);
            try
            {
                var serializedData = JsonSerializer.Serialize(models);
                var bytes = Encoding.UTF8.GetBytes(serializedData);
                writer.Write(bytes);
            }
            catch (Exception)
            {
                throw new InvalidOperationException(path);
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
         * Deserealize file from <path> using binary deserialization in to List<TModel>.
        ----------------------------------------------------------------------------------------------------*/
        private void ReceiveModels()
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            using var reader = new BinaryReader(File.Open(path, FileMode.Open), Encoding.UTF8);
            try
            {
                var bytes = reader.ReadBytes((int)reader.BaseStream.Length);
                var deserializedData = Encoding.UTF8.GetString(bytes);
                modelList = JsonSerializer.Deserialize<List<TModel>>(deserializedData);
            }
            catch (Exception)
            {
                throw new InvalidOperationException(path);
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
                else
                {
                    prop.SetValue(cryptedModel, prop.GetValue(model));
                }
            }
            return cryptedModel;
        }

        /*----------------------------------------------------------------------------------------------------
         * Return a copy of List<TModel> with encrypted/decrypted string properties.
        ----------------------------------------------------------------------------------------------------*/
        private List<TModel> ApplyCryptography(List<TModel> models, CryptographyMode mode)
        {
            var result = new List<TModel>(models.Count);

            models.ForEach(m => result.Add(ApplyCryptography(m, mode)));

            return result;
        }

        /*----------------------------------------------------------------------------------------------------
         * Serealize <modelList> to <path> file by binary serialization.
        ----------------------------------------------------------------------------------------------------*/
        private void SaveChanges()
        {
            if (path == null)
                return;

            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            using var writer = new BinaryWriter(File.Open(path, FileMode.Open), Encoding.UTF8);
            try
            {
                var serializedData = JsonSerializer.Serialize(modelList);
                var bytes = Encoding.UTF8.GetBytes(serializedData);
                writer.Write(bytes);
            }
            catch (Exception)
            {
                throw new InvalidOperationException(path);
            }
        }
    }
}
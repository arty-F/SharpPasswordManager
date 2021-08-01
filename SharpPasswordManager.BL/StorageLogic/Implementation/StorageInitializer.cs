﻿using SharpPasswordManager.BL.Security;
using SharpPasswordManager.DL.DataGenerators;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace SharpPasswordManager.BL.StorageLogic
{
    /// <summary>
    /// Serves to generate random string and datetime prepertiy values for model.
    /// </summary>
    /// <typeparam name="TModel">Any model class.</typeparam>
    public class StorageInitializer<TModel> : IStorageInitializer<TModel> where TModel : class
    {
        private readonly IDataGenerator dataGenerator;
        private readonly ICryptographer cryptographer;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageInitializer{TModel}"/>.
        /// </summary>
        /// <param name="dataGenerator">Using for generate random data.</param>
        /// <param name="cryptographer">Using for data encryption/decryption.</param>
        public StorageInitializer(IDataGenerator dataGenerator, ICryptographer cryptographer = null)
        {
            this.dataGenerator = dataGenerator;
            this.cryptographer = cryptographer;
        }

        /// <summary>
        /// Asynchronously generated list of TModel with properties generated by IDataGenerator and return this collection.
        /// </summary>
        /// <param name="modelsCount">Required TModel count in list.</param>
        public async Task<List<TModel>> GetDataAsync(int modelsCount = 10000)
        {
            return await Task.Run(() => GetData(modelsCount));
        }

        /*----------------------------------------------------------------------------------------------------
         * Create and return List of <TModel>. Properties of <TModel> will be equal to the following values:
         
                - <DataTime> type : All properties will be generate in <dataGenerator.GenerateRandomDate> 
                                    method.

                - <String> type :   Properties with "Login" name will be generate in 
                                    <dataGenerator.GenerateRandomLogin()> method.

                                    Properties with "Password" name will be generate in 
                                    <dataGenerator.GenerateRandomPassword()> method.

                                    All other properties will be generate in 
                                    <dataGenerator.GenerateRandomDescription()> method.
        ----------------------------------------------------------------------------------------------------*/
        /// <summary>
        /// Return List of TModel with properties generated by IDataGenerator.
        /// </summary>
        /// <param name="modelsCount">Required TModel count in list.</param>
        public List<TModel> GetData(int modelsCount = 10000)
        {
            List<TModel> dataList = new List<TModel>();
            dataList.Capacity = modelsCount;
            var genericType = GetType().GetGenericArguments();

            for (int i = 0; i < modelsCount; i++)
            {
                dynamic model = Activator.CreateInstance(genericType[0]);
                foreach (var prop in (model as TModel).GetType().GetProperties())
                {
                    PropertyInfo pInfo = prop;
                    if (pInfo == null || pInfo.PropertyType != typeof(string))
                    {
                        continue;
                    }

                    // Without encryption
                    if (cryptographer == null)
                    {
                        switch (pInfo.Name)
                        {
                            case "Login":
                            {
                                prop.SetValue(model, dataGenerator.GenerateRandomLogin());
                                break;
                            }
                            case "Password":
                            {
                                prop.SetValue(model, dataGenerator.GenerateRandomPassword());
                                break;
                            }
                            case "Date":
                            {
                                prop.SetValue(model, dataGenerator.GenerateRandomDate());
                                break;
                            }
                            default:
                            {
                                prop.SetValue(model, dataGenerator.GenerateRandomUrl());
                                break;
                            }
                        }
                    }
                    // With encryption
                    else
                    {
                        switch (pInfo.Name)
                        {
                            case "Login":
                            {
                                prop.SetValue(model, cryptographer.Encypt(dataGenerator.GenerateRandomLogin()));
                                break;
                            }
                            case "Password":
                            {
                                prop.SetValue(model, cryptographer.Encypt(dataGenerator.GenerateRandomPassword()));
                                break;
                            }
                            case "Date":
                            {
                                prop.SetValue(model, cryptographer.Encypt(dataGenerator.GenerateRandomDate()));
                                break;
                            }
                            default:
                            {
                                prop.SetValue(model, cryptographer.Encypt(dataGenerator.GenerateRandomUrl()));
                                break;
                            }
                        }
                    }
                }
                dataList.Add(model);
            }
            return dataList;
        }
    }
}
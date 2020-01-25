using SharpPasswordManager.DL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Microsoft.CSharp;

namespace SharpPasswordManager.BL
{
    public class StorageInitializer<TModel> where TModel : class
    {
        private readonly IDataGenerator dataGenerator;
        private readonly ICryptographer cryptographer;

        public StorageInitializer(IDataGenerator dataGenerator, ICryptographer cryptographer = null)
        {
            this.dataGenerator = dataGenerator;
            this.cryptographer = cryptographer;
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
        public List<TModel> GetData(int modelsCount)
        {
            List<TModel> dataList = new List<TModel>();
            var genericType = GetType().GetGenericArguments();

            for (int i = 0; i < modelsCount; i++)
            {
                dynamic model = Activator.CreateInstance(genericType[0]);
                foreach (var prop in (model as TModel).GetType().GetProperties())
                {
                    PropertyInfo pInfo = prop as PropertyInfo;
                    if (pInfo != null)
                    {
                        if (pInfo.PropertyType == typeof(DateTime))
                        {
                            prop.SetValue(model, dataGenerator.GenerateRandomDate());
                        }
                        else if (pInfo.PropertyType == typeof(string))
                        {
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
                                    default:
                                    {
                                        prop.SetValue(model, dataGenerator.GenerateRandomDescription());
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
                                    default:
                                    {
                                        prop.SetValue(model, cryptographer.Encypt(dataGenerator.GenerateRandomDescription()));
                                        break;
                                    }
                                }
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

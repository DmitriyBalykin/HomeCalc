﻿using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DbService
{
    public static class DefaultDbContent
    {
        public static Dictionary<string, string> OldTables { get
        {
            return new Dictionary<string,string>{
                {"PURCHASETYPEMODELS" , "(Name TEXT, TypeId INTEGER PRIMARY KEY)"},
                {"PURCHASEMODELS" , "(Name TEXT, Timestamp INTEGER, TotalCost REAL, ItemCost REAL, ItemsNumber REAL, TypeId INTEGER, PurchaseId INTEGER PRIMARY KEY, FOREIGN KEY(TypeId) REFERENCES PURCHASETYPEMODELS(TypeId) ON DELETE CASCADE ON UPDATE CASCADE)"}
            };
        } }
        public static Dictionary<string, string> AlterTables
        {
            get
            {
                return new Dictionary<string, string>{
                {"PURCHASETYPEMODELS" , "(TypeId INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT)"},
                {"PURCHASEMODELS" , "(PurchaseId INTEGER PRIMARY KEY AUTOINCREMENT,Name TEXT, Timestamp INTEGER, TotalCost REAL, ItemCost REAL, ItemsNumber REAL, TypeId INTEGER, FOREIGN KEY(TypeId) REFERENCES PURCHASETYPEMODELS(TypeId) ON DELETE CASCADE ON UPDATE CASCADE)"},
                {"SETTINGMODELS" , "(SettingId INTEGER PRIMARY KEY AUTOINCREMENT, ProfileId INTEGER, SettingName TEXT, SettingValue TEXT)"}
            };
            }
        }

        public static Dictionary<string, string> Tables
        {
            get
            {
                return new Dictionary<string, string>{
                {"PURCHASETYPEMODELS" , "(TypeId INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT)"},
                {"PURCHASEMODELS" , "(PurchaseId INTEGER PRIMARY KEY AUTOINCREMENT,Name TEXT, Timestamp INTEGER, TotalCost REAL, ItemCost REAL, ItemsNumber REAL, TypeId INTEGER, FOREIGN KEY(TypeId) REFERENCES PURCHASETYPEMODELS(TypeId) ON DELETE CASCADE ON UPDATE CASCADE)"},
                {"SETTINGMODELS" , "(SettingId INTEGER PRIMARY KEY AUTOINCREMENT, ProfileId INTEGER, SettingName TEXT, SettingValue TEXT)"}
            };
            }
        }

        public static List<DbItem<PurchaseTypeModel>> Values
        {
            get
            {
            return new List<DbItem<PurchaseTypeModel>>(){
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPEMODELS" , Value = new PurchaseTypeModel{ Name = "Еда" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPEMODELS" , Value = new PurchaseTypeModel{ Name = "Хозяйственные товары" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPEMODELS" , Value = new PurchaseTypeModel{ Name = "Автомобиль" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPEMODELS" , Value = new PurchaseTypeModel{ Name = "Квартира" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPEMODELS" , Value = new PurchaseTypeModel{ Name = "Снаряжение" }},
            };
            }
        }

        public static List<SettingsStorageModel> Settings
        {
            get
            {
                var list = new List<SettingsStorageModel>();
                list.Add(new SettingsStorageModel 
                {
                    ProfileId = 0,
                    SettingId = 0,
                    SettingName = "AutoUpdate",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 1,
                    SettingName = "AutoUpdateCheck",
                    SettingValue = "true"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 2,
                    SettingName = "BackupPath",
                    SettingValue = ""
                });

                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 3,
                    SettingName = "PurchaseSubType",
                    SettingValue = ""
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 4,
                    SettingName = "PurchaseComment",
                    SettingValue = ""
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 5,
                    SettingName = "PurchaseRate",
                    SettingValue = ""
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 6,
                    SettingName = "StoreName",
                    SettingValue = ""
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 7,
                    SettingName = "StoreRate",
                    SettingValue = ""
                });





                return list;
            }
        }
    }

    public class DbItem<T>
    {
        public string Table { get; set; }
        public T Value { get; set; }
    }
}
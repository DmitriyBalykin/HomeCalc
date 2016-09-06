using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DbService
{
    public static class DefaultDbContent
    {

        public static Dictionary<int, Dictionary<string, string>> Tables
        {
            get
            {
                return new Dictionary<int, Dictionary<string, string>>{
                    {0, new Dictionary<string, string>{
                        {"PURCHASETYPEMODELS" , "(TypeId INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT)"},
                        {"PURCHASEMODELS" , "(PurchaseId INTEGER PRIMARY KEY AUTOINCREMENT,Name TEXT, Timestamp INTEGER, TotalCost REAL, ItemCost REAL, ItemsNumber REAL, TypeId INTEGER)"},
                        {"SETTINGMODELS" , "(SettingId INTEGER PRIMARY KEY AUTOINCREMENT, ProfileId INTEGER, SettingName TEXT, SettingValue TEXT)"}    
                    }},
                    {1, new Dictionary<string, string>{
                        {"PURCHASETYPE" , "(TypeId INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT)"},
                        {"PURCHASESUBTYPE" , "(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT)"},
                        {"STORE" , "(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT)"},
                        {"PURCHASE" , "(PurchaseId INTEGER PRIMARY KEY AUTOINCREMENT,Name TEXT, TypeId INTEGER, SubTypeId INTEGER, IsMonthly BOOLEAN)"},
                        {"PURCHASEITEM" , "(PurchaseItemId INTEGER PRIMARY KEY AUTOINCREMENT, PurchaseId INTEGER, Timestamp INTEGER, TotalCost REAL, ItemCost REAL, ItemsNumber REAL, StoreId INTEGER)"},
                        {"SETTING" , "(SettingId INTEGER PRIMARY KEY AUTOINCREMENT, ProfileId INTEGER, SettingName TEXT, SettingValue TEXT)"}    
                    }}
                
            };
            }
        }

        public static List<DbItem<PurchaseTypeModel>> Values
        {
            get
            {
            return new List<DbItem<PurchaseTypeModel>>(){
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPE" , Value = new PurchaseTypeModel{ Name = "Еда" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPE" , Value = new PurchaseTypeModel{ Name = "Хозяйственные товары" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPE" , Value = new PurchaseTypeModel{ Name = "Автомобиль" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPE" , Value = new PurchaseTypeModel{ Name = "Квартира" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPE" , Value = new PurchaseTypeModel{ Name = "Снаряжение" }},
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
                    SettingName = "ShowPurchaseSubType",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 4,
                    SettingName = "ShowPurchaseComment",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 5,
                    SettingName = "ShowPurchaseRate",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 6,
                    SettingName = "ShowMonthlyPurchaseRate",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 6,
                    SettingName = "ShowStoreName",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 6,
                    SettingName = "ShowStoreComment",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 7,
                    SettingName = "ShowStoreRate",
                    SettingValue = "false"
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
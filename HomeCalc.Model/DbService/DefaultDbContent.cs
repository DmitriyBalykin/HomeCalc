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
        public static Dictionary<string, string> Tables { get
        {
            return new Dictionary<string,string>{
                {"PURCHASETYPEMODELS" , "(TypeId INTEGER PRIMARY KEY, Name TEXT)"},
                {"PURCHASEMODELS" , "(PurchaseId INTEGER PRIMARY KEY, Name TEXT, Timestamp INTEGER, TotalCost REAL, ItemCost REAL, ItemsNumber REAL, TypeId INTEGER, FOREIGN KEY(TypeId) REFERENCES PURCHASETYPEMODELS(TypeId) ON DELETE CASCADE ON UPDATE CASCADE)"},
                {"SETTINGS" , "(SettingsProdileId INTEGER PRIMARY KEY, AutoWindowPosition INTEGER, AutoWindowSize INTEGER, ClearFieldsOnSave INTEGER, ResetCalculation INTEGER, BackupPath TEXT)"}
            };
        } }
        public static List<DbItem<object>> Values
        {
            get
            {
            return new List<DbItem<object>>(){
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPEMODELS" , Value = new PurchaseTypeModel{ Name = "Їжа" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPEMODELS" , Value = new PurchaseTypeModel{ Name = "Господарчі товари" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPEMODELS" , Value = new PurchaseTypeModel{ Name = "Автомобіль" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPEMODELS" , Value = new PurchaseTypeModel{ Name = "Квартира" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPEMODELS" , Value = new PurchaseTypeModel{ Name = "Медицина" }}
            };
            }
        }

        public static List<DbItem<SettingsModel>> Settings
        {
            get
            {
                return new List<DbItem<SettingsModel>>(){
                new DbItem<SettingsModel>
                {
                    Table = "SETTINGS" ,
                    Value = new SettingsModel
                    {
                        AutoWindowPosition = 1,
                        AutoWindowSize = 1,
                        ClearFieldsOnSave = 1,
                        ResetCalculation = 0,
                        BackupPath = string.Empty
                    }
                },
            };}
        }
    }

    public class DbItem<T>
    {
        public string Table { get; set; }
        public T Value { get; set; }
    }
}
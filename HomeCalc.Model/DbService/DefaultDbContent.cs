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
                {"PURCHASEMODELS" , "(PurchaseId INTEGER PRIMARY KEY, Name TEXT, Timestamp TEXT, TotalCost REAL, ItemCost REAL, ItemsNumber REAL, TypeId INTEGER, FOREIGN KEY(TypeId) REFERENCES PURCHASETYPEMODELS(TypeId))"}
            };
        } }
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
    }

    public class DbItem<T>
    {
        public string Table { get; set; }
        public T Value { get; set; }
    }
}
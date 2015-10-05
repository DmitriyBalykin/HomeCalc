using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DbService
{
    public class DataService
    {
        private static DataService instance;
        private static object monitor = new object();
        private DataService()
        { }
        public static DataService GetInstance()
        {
            lock (monitor)
            {
                if (instance == null)
                {
                    instance = new DataService();
                }
            }
            return instance;
        }
        public bool SaveSettings(SettingsModel settings)
        {
            using (var db = new StorageContext())
            {

            }
            return false;
        }
        public SettingsModel LoadSettings()
        {
            SettingsModel settings = null;
            using (var db = new StorageContext())
            {

            }
            return settings;
        }
        public bool SavePurchase(PurchaseModel purchase)
        {
            bool result = false;
            using (var db = new StorageContext())
            {
                try
                {
                    db.Purchase.Add(purchase);
                    db.SaveChanges();
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                }
                
            }
            return result;
        }
        public bool SavePurchaseType(PurchaseTypeModel purchaseType)
        {
            using (var db = new StorageContext())
            {

            }
            return false;
        }
        public PurchaseModel LoadPurchase(int id)
        {
            PurchaseModel purchase = null;
            using (var db = new StorageContext())
            {

            }
            return purchase;
        }
        public IEnumerable<PurchaseModel> LoadPurchaseList(object filter)
        {
            IEnumerable<PurchaseModel> list = null;
            using (var db = new StorageContext())
            {

            }
            return list;
        }
        public IEnumerable<PurchaseTypeModel> LoadPurchaseTypeList()
        {
            IEnumerable<PurchaseTypeModel> list = null;
            using (var db = new StorageContext())
            {

            }
            return list;
        }
    }
}

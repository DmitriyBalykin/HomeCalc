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
        private IDatabaseManager dbManager;
        private static object monitor = new object();
        private DataService(IDatabaseManager dbManager)
        {
            this.dbManager = dbManager;
        }
        public static DataService GetInstance()
        {
            lock (monitor)
            {
                if (instance == null)
                {
                    instance = new DataService(new SQLiteManager());
                }
            }
            return instance;
        }
        public bool SaveSettings(SettingsModel settings)
        {
            using (var db = dbManager.GetContext())
            {

            }
            return false;
        }
        public SettingsModel LoadSettings()
        {
            SettingsModel settings = null;
            using (var db = dbManager.GetContext())
            {

            }
            return settings;
        }
        public bool SavePurchase(PurchaseModel purchase)
        {
            bool result = false;
            using (var db = dbManager.GetContext())
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
            using (var db = dbManager.GetContext())
            {

            }
            return false;
        }
        public PurchaseModel LoadPurchase(int id)
        {
            PurchaseModel purchase = null;
            using (var db = dbManager.GetContext())
            {

            }
            return purchase;
        }
        public IEnumerable<PurchaseModel> LoadPurchaseList(object filter)
        {
            IEnumerable<PurchaseModel> list = null;
            using (var db = dbManager.GetContext())
            {

            }
            return list;
        }
        public IEnumerable<PurchaseTypeModel> LoadPurchaseTypeList()
        {
            IEnumerable<PurchaseTypeModel> list = null;
            using (var db = dbManager.GetContext())
            {
                try
                {
                    list = db.PurchaseType.ToList();
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }
            return list;
        }
    }
}

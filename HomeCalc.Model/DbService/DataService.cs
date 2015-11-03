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
        public bool SavePurchaseBulk(IEnumerable<PurchaseModel> purchases)
        {
            bool result = false;
            using (var db = dbManager.GetContext())
            {
                try
                {
                    db.Purchase.AddRange(purchases);
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
                db.PurchaseType.Add(purchaseType);
                db.SaveChanges();
            }
            return false;
        }
        public PurchaseModel LoadPurchase(int id)
        {
            PurchaseModel purchase = null;
            using (var db = dbManager.GetContext())
            {
                purchase = db.Purchase.Find(id);
            }
            return purchase;
        }
        public IList<PurchaseModel> LoadPurchaseList(Func<PurchaseModel, bool> request)
        {
            IList<PurchaseModel> list = null;
            using (var db = dbManager.GetContext())
            {
                list = db.Purchase.Where(request).ToList();
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
        public bool UpdatePurchase(PurchaseModel purchase)
        {
            bool result = false;
            using (var db = dbManager.GetContext())
            {
                try
                {
                    var storedPurchase = db.Purchase.Find(purchase.PurchaseId);
                    if (storedPurchase != null)
                    {
                        storedPurchase.Name = purchase.Name;
                        storedPurchase.ItemsNumber = purchase.ItemsNumber;
                        storedPurchase.ItemCost = purchase.ItemCost;
                        storedPurchase.TotalCost = purchase.TotalCost;
                        storedPurchase.Timestamp = purchase.Timestamp;
                        db.SaveChanges();
                        result = true;
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }
        public bool DeletePurchase(long purchaseId)
        {
            bool result = false;
            using (var db = dbManager.GetContext())
            {
                try
                {
                    var storedPurchase = db.Purchase.Find(purchaseId);
                    if (storedPurchase != null)
                    {
                        db.Purchase.Remove(storedPurchase);
                        db.SaveChanges();
                        result = true;
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }
        public bool UpdatePurchaseType(PurchaseTypeModel type)
        {
            bool result = false;
            using (var db = dbManager.GetContext())
            {
                try
                {
                    var storedType = db.PurchaseType.Find(type.TypeId);
                    if (storedType != null)
                    {
                        storedType.Name = type.Name;
                        db.SaveChanges();
                        result = true;
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }
        public bool DeletePurchaseType(PurchaseTypeModel type)
        {
            bool result = false;
            using (var db = dbManager.GetContext())
            {
                try
                {
                    var storedType = db.PurchaseType.Find(type.TypeId);
                    if (storedType != null)
                    {
                        db.PurchaseType.Remove(storedType);
                        db.SaveChanges();
                        result = true;
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }
    }
}

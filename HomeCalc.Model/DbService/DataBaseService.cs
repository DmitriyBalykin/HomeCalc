﻿using HomeCalc.Core.LogService;
using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DbService
{
    public class DataBaseService
    {

        private Logger logger = LogService.GetLogger();
        private static DataBaseService instance;
        private IDatabaseManager dbManager;
        private static object monitor = new object();
        private DataBaseService(IDatabaseManager dbManager)
        {
            logger.Info("New database instance initiated.");
            this.dbManager = dbManager;
        }
        public static DataBaseService GetInstance()
        {
            lock (monitor)
            {
                if (instance == null)
                {
                    instance = new DataBaseService(new SQLiteManager());
                }
            }
            return instance;
        }
        public async Task<bool> SaveSettings(SettingsStorageModel settings)
        {
            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                {
                    using (var command = db.Connection.CreateCommand())
                    {
                        command.CommandText = string.Format("INSERT INTO SETTINGS ({0}, {1})", settings.SettingName, settings.SettingValue);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"SaveSettings\": {0}", ex.Message);
            }

            return result;
        }
        public async Task<IEnumerable<SettingsStorageModel>> LoadSettings()
        {
            IEnumerable<SettingsStorageModel> settings = new List<SettingsStorageModel>();
            try
            {
                using (var db = dbManager.GetConnection())
                {
                    using (var command = db.Connection.CreateCommand())
                    {
                        command.CommandText = string.Format("SELECT * FROM SETTINGS");
                        DbDataReader dbDataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                        while (var result = dbDataReader.)
                        {
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadSettings\": {0}", ex.Message);
            }

            return settings;
        }
        public async Task<bool> AddPurchase(PurchaseModel purchase)
        {
            bool result = false;
            using (var db = dbManager.GetConnection())
            {
                try
                {
                    var query = string.Format(
                        "INSERT INTO PURCHASEMODELS VALUES({0}, {1}, {2}, {3}, {4}, {5})",
                        purchase.Name, purchase.Timestamp, purchase.TotalCost, purchase.ItemCost, purchase.ItemsNumber, purchase.TypeId);

                    await new SQLiteCommand(query, db.Connection).ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }
        public async Task<bool> SavePurchaseBulk(IEnumerable<PurchaseModel> purchases)
        {
            bool result = false;
            using (var db = dbManager.GetConnection())
            {
                try
                {
                    var transaction = db.Connection.BeginTransaction();
                    
                    foreach(var purchase in purchases)
                    {
                        var query = string.Format(
                        "INSERT INTO PURCHASEMODELS VALUES({0}, {1}, {2}, {3}, {4}, {5})",
                        purchase.Name, purchase.Timestamp, purchase.TotalCost, purchase.ItemCost, purchase.ItemsNumber, purchase.TypeId);

                        await new SQLiteCommand(query, transaction.Connection).ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                    transaction.Commit();
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
            bool result = false;
            using (var db = dbManager.GetConnection())
            {
                try
                {
                    db.PurchaseType.Add(purchaseType);
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
        public PurchaseModel LoadPurchase(int id)
        {
            PurchaseModel purchase = null;
            using (var db = dbManager.GetConnection())
            {
                purchase = db.Purchase.Find(id);
            }
            return purchase;
        }
        public IList<PurchaseModel> LoadPurchaseList(Func<PurchaseModel, bool> request)
        {
            IList<PurchaseModel> list = null;
            using (var db = dbManager.GetConnection())
            {
                list = db.Purchase.Where(request).ToList();
            }
            return list;
        }
        public IList<PurchaseModel> LoadCompletePurchaseList()
        {
            IList<PurchaseModel> list = null;
            using (var db = dbManager.GetConnection())
            {
                list = db.Purchase.ToList();
            }
            return list;
        }
        public IEnumerable<PurchaseTypeModel> LoadPurchaseTypeList()
        {
            IEnumerable<PurchaseTypeModel> list = null;
            using (var db = dbManager.GetConnection())
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
            using (var db = dbManager.GetConnection())
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
        public bool RemovePurchase(long purchaseId)
        {
            bool result = false;
            using (var db = dbManager.GetConnection())
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
            using (var db = dbManager.GetConnection())
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
            using (var db = dbManager.GetConnection())
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

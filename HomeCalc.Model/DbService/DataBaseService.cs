using HomeCalc.Core.LogService;
using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
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
                        using (var transaction = db.Connection.BeginTransaction(IsolationLevel.Serializable))
                        {
                            if (settings.SettingId == 0)
                            {
                                command.CommandText = string.Format("INSERT OR REPLACE INTO SETTINGS ({0}, {1}, {2})", settings.ProfileId, settings.SettingName, settings.SettingValue);
                            }
                            else
                            {
                                command.CommandText = string.Format("UPDATE SETTINGS SET ProfileId = {0}, SettingName = {1}, SettingValue {2} WHERE SettingId = {3}", settings.ProfileId, settings.SettingName, settings.SettingValue, settings.SettingId);
                            }
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                            transaction.Commit();
                        }
                        
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
            var settings = new List<SettingsStorageModel>();
            try
            {
                using (var db = dbManager.GetConnection())
                {
                    using (var command = db.Connection.CreateCommand())
                    {
                        command.CommandText = string.Format("SELECT * FROM SETTINGS");
                        DbDataReader dbDataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                        //{"SETTINGMODELS" , "(ProfileId INTEGER, SettingName TEXT, SettingValue TEXT, SettingId INTEGER PRIMARY KEY)"}
                        while (dbDataReader.HasRows && dbDataReader.Read())
                        {
                            settings.Add(new SettingsStorageModel()
                            {
                                SettingId = dbDataReader.GetInt64(0),
                                ProfileId = dbDataReader.GetInt64(1),
                                SettingName = dbDataReader.GetString(2),
                                SettingValue = dbDataReader.GetString(3)
                            });
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
        public async Task<bool> SavePurchase(PurchaseModel purchase)
        {
            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                {
                    using (var command = db.Connection.CreateCommand())
                    {
                        if (purchase.PurchaseId == 0)
                        {
                            command.CommandText = string.Format(
                            "INSERT OR REPLACE INTO PURCHASEMODELS VALUES({0}, {1}, {2}, {3}, {4}, {5})",
                            purchase.Name, purchase.Timestamp, purchase.TotalCost, purchase.ItemCost, purchase.ItemsNumber, purchase.TypeId);
                        }
                        else
                        {
                            command.CommandText = string.Format(
                            "UPDATE PURCHASEMODELS SET Name = {0}, Timestamp = {1}, TotalCost = {2}, ItemCost = {3}, ItemsNumber = {4}, TypeId = {5} WHERE PurchaseId = {6}",
                            purchase.Name, purchase.Timestamp, purchase.TotalCost, purchase.ItemCost, purchase.ItemsNumber, purchase.TypeId, purchase.PurchaseId);
                        }

                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"AddPurchase\": {0}", ex.Message);
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
                    using (var transaction = db.Connection.BeginTransaction())
                    {
                        using (var command = db.Connection.CreateCommand())
                        {
                            foreach (var purchase in purchases)
                            {
                                command.CommandText = string.Format(
                                "INSERT INTO PURCHASEMODELS VALUES({0}, {1}, {2}, {3}, {4}, {5})",
                                purchase.Name, purchase.Timestamp, purchase.TotalCost, purchase.ItemCost, purchase.ItemsNumber, purchase.TypeId);

                                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                            }
                            transaction.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    logger.Error("Exception during execution method \"SavePurchaseBulk\": {0}", ex.Message);
                }

            }
            return result;
        }
        public async Task<bool> SavePurchaseType(PurchaseTypeModel purchaseType)
        {

            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                {
                    using (var command = db.Connection.CreateCommand())
                    {
                        if (purchaseType.TypeId == 0)
                        {
                            command.CommandText = string.Format("INSERT OR REPLACE INTO PURCHASETYPEMODELS ({0})", purchaseType.Name);
                        }
                        else
                        {
                            command.CommandText = string.Format("INSERT OR REPLACE INTO PURCHASETYPEMODELS ({0})", purchaseType.Name);
                        }
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"SavePurchaseType\": {0}", ex.Message);
            }
            return result;
        }
        public async Task<PurchaseModel> LoadPurchase(int id)
        {
            PurchaseModel purchase = null;
            try
            {
                using (var db = dbManager.GetConnection())
                {
                    using (var command = db.Connection.CreateCommand())
                    {
                        command.CommandText = string.Format("SELECT * FROM PURCHASEMODELS WHERE PurchaseId = {0}", id);
                        var dbReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                        if (dbReader.HasRows && dbReader.Read())
                        {
                            //{"PURCHASEMODELS" , "(PurchaseId INTEGER PRIMARY KEY AUTOINCREMENT,Name TEXT, Timestamp INTEGER, TotalCost REAL, ItemCost REAL, ItemsNumber REAL, TypeId INTEGER, FOREIGN KEY(TypeId) REFERENCES PURCHASETYPEMODELS(TypeId) ON DELETE CASCADE ON UPDATE CASCADE)"},
                            purchase = new PurchaseModel
                            {
                                Name = dbReader.GetString(1),
                                Timestamp = dbReader.GetInt64(2),
                                TotalCost = dbReader.GetDouble(3),
                                ItemCost = dbReader.GetDouble(4),
                                ItemsNumber = dbReader.GetDouble(5),
                                TypeId = dbReader.GetInt64(6)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadPurchase\": {0}", ex.Message);
            }
            return purchase;
        }
        public IList<PurchaseModel> LoadPurchaseList(Func<PurchaseModel, bool> request)
        {
            IList<PurchaseModel> list = null;
            try
            {
            using (var db = dbManager.GetConnection())
            {
                list = db.Purchase.Where(request).ToList();
            }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadPurchaseList\": {0}", ex.Message);
            }
            return list;
        }
        public IList<PurchaseModel> LoadCompletePurchaseList()
        {
            IList<PurchaseModel> list = null;
            try
            {
                using (var db = dbManager.GetConnection())
                {
                    list = db.Purchase.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadCompletePurchaseList\": {0}", ex.Message);
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
                catch (Exception ex)
                {
                    logger.Error("Exception during execution method \"LoadPurchaseTypeList\": {0}", ex.Message);
                }
            }
            return list;
        }
        // Use Save purchase instead
        //public bool UpdatePurchase(PurchaseModel purchase)
        //{
        //    bool result = false;
        //    using (var db = dbManager.GetConnection())
        //    {
        //        try
        //        {
        //            var storedPurchase = db.Purchase.Find(purchase.PurchaseId);
        //            if (storedPurchase != null)
        //            {
        //                storedPurchase.Name = purchase.Name;
        //                storedPurchase.ItemsNumber = purchase.ItemsNumber;
        //                storedPurchase.ItemCost = purchase.ItemCost;
        //                storedPurchase.TotalCost = purchase.TotalCost;
        //                storedPurchase.Timestamp = purchase.Timestamp;
        //                db.SaveChanges();
        //                result = true;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            result = false;
        //            logger.Error("Exception during execution method \"UpdatePurchase\": {0}", ex.Message);
        //        }
        //    }
        //    return result;
        //}
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
                catch (Exception ex)
                {
                    result = false;
                    logger.Error("Exception during execution method \"RemovePurchase\": {0}", ex.Message);
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
                catch (Exception ex)
                {
                    result = false;
                    logger.Error("Exception during execution method \"UpdatePurchaseType\": {0}", ex.Message);
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
                catch (Exception ex)
                {
                    result = false;
                    logger.Error("Exception during execution method \"DeletePurchaseType\": {0}", ex.Message);
                }
            }
            return result;
        }
    }
}

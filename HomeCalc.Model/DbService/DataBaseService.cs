using HomeCalc.Core.LogService;
using HomeCalc.Model.DataModels;
using HomeCalc.Model.DbConnectionWrappers;
using HomeCalc.Presentation.Models;
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
        public async Task<bool> SaveSettings(SettingsStorageModel settings, StorageConnection connection = null)
        {
            bool result = false;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                using (var transaction = db.Connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    if (settings.SettingId == 0)
                    {
                        command.CommandText = string.Format("INSERT OR REPLACE INTO SETTINGS(ProfileId, SettingName, SettingValue) VALUES ({0}, {1}, {2})", settings.ProfileId, settings.SettingName, settings.SettingValue);
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE SETTINGS SET ProfileId = {0}, SettingName = {1}, SettingValue {2} WHERE SettingId = {3}", settings.ProfileId, settings.SettingName, settings.SettingValue, settings.SettingId);
                    }
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    transaction.Commit();
                }
                result = true;
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"SaveSettings\": {0}", ex.Message);
            }

            return result;
        }
        public async Task<IEnumerable<SettingsStorageModel>> LoadSettings(StorageConnection connection = null)
        {
            var settings = new List<SettingsStorageModel>();
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
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
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadSettings\": {0}", ex.Message);
            }

            return settings;
        }
        public async Task<bool> SavePurchase(PurchaseModel purchase, StorageConnection connection = null)
        {
            bool result = false;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    if (purchase.PurchaseId == 0)
                    {
                        command.CommandText = string.Format(
                        "INSERT OR REPLACE INTO PURCHASEMODELS(Name, Timestamp, TotalCost, ItemCost, ItemsNumber, TypeId) VALUES (\"{0}\", {1}, {2}, {3}, {4}, {5})",
                        purchase.Name, purchase.Timestamp, purchase.TotalCost, purchase.ItemCost, purchase.ItemsNumber, purchase.TypeId);
                    }
                    else
                    {
                        command.CommandText = string.Format(
                        "UPDATE PURCHASEMODELS SET Name = \"{0}\", Timestamp = {1}, TotalCost = {2}, ItemCost = {3}, ItemsNumber = {4}, TypeId = {5} WHERE PurchaseId = {6}",
                        purchase.Name, purchase.Timestamp, purchase.TotalCost, purchase.ItemCost, purchase.ItemsNumber, purchase.TypeId, purchase.PurchaseId);
                    }

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"AddPurchase\": {0}", ex.Message);
            }
            return result;
        }
        public async Task<bool> SavePurchaseBulk(IEnumerable<PurchaseModel> purchases, StorageConnection connection = null)
        {
            bool result = false;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var transaction = db.Connection.BeginTransaction())
                using (var command = db.Connection.CreateCommand())
                {
                    foreach (var purchase in purchases)
                    {
                        command.CommandText = string.Format(
                        "INSERT INTO PURCHASEMODELS(Name, Timestamp, TotalCost, ItemCost, ItemNumber, TypeId) VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                        purchase.Name, purchase.Timestamp, purchase.TotalCost, purchase.ItemCost, purchase.ItemsNumber, purchase.TypeId);

                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"SavePurchaseBulk\": {0}", ex.Message);
            }
            return result;
        }
        public async Task<bool> SavePurchaseType(PurchaseTypeModel purchaseType, StorageConnection connection = null)
        {

            bool result = false;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    if (purchaseType.TypeId == 0)
                    {
                        command.CommandText = string.Format("INSERT OR REPLACE INTO PURCHASETYPEMODELS (Name) VALUES ({0})", purchaseType.Name);
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE PURCHASETYPEMODELS SET Name = \"{0}\" WHERE TypeId = {1}", purchaseType.Name, purchaseType.TypeId);
                    }
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"SavePurchaseType\": {0}", ex.Message);
            }
            return result;
        }
        public async Task<PurchaseModel> LoadPurchase(int id, StorageConnection connection = null)
        {
            PurchaseModel purchase = null;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM PURCHASEMODELS WHERE PurchaseId = {0}", id);
                    var dbReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    if (dbReader.HasRows && dbReader.Read())
                    {
                        //{"PURCHASEMODELS" , "(PurchaseId INTEGER PRIMARY KEY AUTOINCREMENT,Name TEXT, Timestamp INTEGER, TotalCost REAL, ItemCost REAL, ItemsNumber REAL, TypeId INTEGER, FOREIGN KEY(TypeId) REFERENCES PURCHASETYPEMODELS(TypeId) ON DELETE CASCADE ON UPDATE CASCADE)"},
                        purchase = new PurchaseModel
                        {
                            PurchaseId = dbReader.GetInt64(0),
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
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadPurchase\": {0}", ex.Message);
            }
            return purchase;
        }
        public async Task<List<PurchaseModel>> LoadPurchaseList(SearchRequestModel filter, StorageConnection connection = null)
        {
            var list = new List<PurchaseModel>();
            try
            {//{"PURCHASEMODELS" , "(PurchaseId INTEGER PRIMARY KEY AUTOINCREMENT,Name TEXT, Timestamp INTEGER, TotalCost REAL, ItemCost REAL, ItemsNumber REAL, TypeId INTEGER, FOREIGN KEY(TypeId) REFERENCES PURCHASETYPEMODELS(TypeId) ON DELETE CASCADE ON UPDATE CASCADE)"},
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    string queue = "SELECT * FROM PURCHASEMODELS";
                    if (filter.SearchByName)
                    {
                        queue = string.Format("{0} WHERE Name = {1} ", queue, filter.Name);
                    }
                    if (filter.SearchByType)
                    {
                        queue = string.Format("{0} WHERE TypeId = {1} ", queue, filter.TypeId);
                    }
                    if (filter.SearchByDate)
                    {
                        queue = string.Format("{0} WHERE Timestamp > {1} AND Timestamp <= {2} ", queue, filter.DateStart.Ticks, filter.DateEnd.Ticks);
                    }
                    if (filter.SearchByCost)
                    {
                        queue = string.Format("{0} WHERE TotalCost > {1} AND TotalCost <= {2} ", queue, filter.CostStart, filter.CostEnd);
                    }
                    command.CommandText = queue.Replace("  ", " AND ");

                    var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    
                    while (dataReader.Read())
                    {
                        list.Add(new PurchaseModel
                        {
                            PurchaseId = dataReader.GetInt64(0),
                            Name = dataReader.GetString(1),
                            Timestamp = dataReader.GetInt64(2),
                            TotalCost = dataReader.GetDouble(3),
                            ItemCost = dataReader.GetDouble(4),
                            ItemsNumber = dataReader.GetDouble(5),
                            TypeId = dataReader.GetInt64(6)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadPurchaseList\": {0}", ex.Message);
            }
            return list;
        }
        public async Task<List<PurchaseModel>> LoadCompletePurchaseList(StorageConnection connection = null)
        {
            return await LoadPurchaseList(new SearchRequestModel(), connection);
        }
        public async Task<IEnumerable<PurchaseTypeModel>> LoadPurchaseTypeList(StorageConnection connection = null)
        {
            var list = new List<PurchaseTypeModel>();
            try
            {
                using(var db = connection ?? dbManager.GetConnection())
                using(var command = db.Connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM PURCHASETYPEMODELS";
                    var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    while (dataReader.Read())
                    {
                        list.Add(new PurchaseTypeModel 
                        {
                            TypeId = dataReader.GetInt64(0),
                            Name = dataReader.GetString(1)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadPurchaseTypeList\": {0}", ex.Message);
            }
            return list;
        }
        // Use Save purchase instead
        //public bool UpdatePurchase(PurchaseModel purchase)
        //{
        //    bool result = false;
        //    using (var db = connection ?? dbManager.GetConnection())
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
        public async Task<bool> RemovePurchase(long purchaseId, StorageConnection connection = null)
        {
            bool result = false;
            try
            {
                using(var db = connection ?? dbManager.GetConnection())
                using(var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM PURCHASEMODELS WHERE PurchaseTypeId = {0}", purchaseId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"RemovePurchase\": {0}", ex.Message);
            }
            
            return result;
        }
        // Use SavePurchaseTypeInstead
        //public bool UpdatePurchaseType(PurchaseTypeModel type)
        //{
        //    bool result = false;
        //    using (var db = connection ?? dbManager.GetConnection())
        //    {
        //        try
        //        {
        //            var storedType = db.PurchaseType.Find(type.TypeId);
        //            if (storedType != null)
        //            {
        //                storedType.Name = type.Name;
        //                db.SaveChanges();
        //                result = true;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            result = false;
        //            logger.Error("Exception during execution method \"UpdatePurchaseType\": {0}", ex.Message);
        //        }
        //    }
        //    return result;
        //}
        public async Task<bool> DeletePurchaseType(PurchaseTypeModel type, StorageConnection connection = null)
        {
            bool result = false;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM PURCHASETYPEMODELS WHERE TypeId = {0}", type.TypeId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"DeletePurchaseType\": {0}", ex.Message);
            }

            return result;
        }
    }
}

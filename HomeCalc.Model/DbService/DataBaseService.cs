using HomeCalc.Core.LogService;
using HomeCalc.Core.Helpers;
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
using System.Globalization;

namespace HomeCalc.Model.DbService
{
    public class DataBaseService
    {

        private Logger logger = LogService.GetLogger();
        private static DataBaseService instance;
        private IDatabaseManager dbManager;
        private static object monitor = new object();

        private IFormatProvider formatCulture = CultureInfo.CreateSpecificCulture("en-US");

        private DataBaseService()
        {
            logger.Info("New database service instance initiated.");
            dbManager = SQLiteManager.GetInstance();

            InitializeDbScheme();
            InitializeDbContent();
        }
        public static DataBaseService GetInstance()
        {
            lock (monitor)
            {
                if (instance == null)
                {
                    instance = new DataBaseService();
                }
            }
            return instance;
        }

        #region Initialization
        private void InitializeDbContent()
        {
            foreach (var value in DefaultDbContent.Values)
            {
                switch (value.Table)
                {
                    case "PURCHASETYPE":
                        var valueExist = LoadPurchaseTypeList(dbManager.GetConnection(true)).Result.Any(p => p.Name == value.Value.Name);
                        if (!valueExist)
                        {
                            var result = SavePurchaseType(new DataModels.PurchaseTypeModel { Name = value.Value.Name }, dbManager.GetConnection(true)).Result;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void InitializeDbScheme()
        {
            try
            {
                using (var connection = dbManager.GetConnection(true))
                using (var transaction = connection.Connection.BeginTransaction())
                using (var command = connection.Connection.CreateCommand())
                {
                    command.CommandText = "PRAGMA user_version";
                    var reader = command.ExecuteReader();
                    reader.Read();
                    var db_version = reader.GetInt32(0);
                    reader.Close();
                    switch (db_version)
                    {
                        case 0:
                            //create common tables
                            command.CommandText =
                                    "create table if not exists PURCHASESUBTYPE (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT);" +
                                    "create table if not exists STORE (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT);" +
                                    "create table if not exists PURCHASE (PurchaseId INTEGER PRIMARY KEY AUTOINCREMENT,Name TEXT, TypeId INTEGER, SubTypeId INTEGER, IsMonthly BOOLEAN);" +
                                    "create table if not exists PURCHASEITEM (PurchaseItemId INTEGER PRIMARY KEY AUTOINCREMENT, PurchaseId INTEGER, Timestamp INTEGER, TotalCost REAL, ItemCost REAL, ItemsNumber REAL, StoreId INTEGER);";
                                command.ExecuteNonQuery();
                            if (IsTableExists("PURCHASETYPEMODELS") && IsTableExists("SETTINGMODELS") && IsTableExists("PURCHASEMODELS"))
                            {
                                //alter existed tables
                                command.CommandText =
                                    "ALTER TABLE PURCHASETYPEMODELS RENAME TO PURCHASETYPE;" +
                                    "ALTER TABLE SETTINGMODELS RENAME TO SETTING;"+
                                    "INSERT INTO PURCHASE (PurchaseId, Name, TypeId) SELECT DISTINCT PurchaseId,Name, TypeId FROM PURCHASEMODELS;"+
                                    "INSERT INTO PURCHASEITEM (PurchaseId, Timestamp, TotalCost, ItemCost, ItemsNumber) SELECT p.PurchaseId, Timestamp, TotalCost, ItemCost, ItemsNumber FROM PURCHASEMODELS pm JOIN PURCHASE p on pm.Name=p.name;"+
                                    "DROP TABLE PURCHASEMODELS";
                                    command.ExecuteNonQuery();
                            }
                            else
                            {
                                //else create new tables
                                command.CommandText =
                                    "create table if not exists PURCHASETYPE (TypeId INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT);"+
                                    "create table if not exists SETTING (SettingId INTEGER PRIMARY KEY AUTOINCREMENT, ProfileId INTEGER, SettingName TEXT, SettingValue TEXT);";
                                command.ExecuteNonQuery();
                            }

                            command.CommandText = "PRAGMA user_version=1;";
                            command.ExecuteNonQuery();
                            break;
                        default:
                            return;
                    }
                    transaction.Commit();
                }
            }
            catch (SQLiteException)
            {
                throw new Exception("Database initialization failed");
            }
        }

        private bool IsTableExists(string table)
        {
            try
            {
                using (var connection = dbManager.GetConnection(true))
                using (var command = connection.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM {0}", table);
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SQLiteException)
            {
                return false;
            }
        }
        #endregion
        public async Task<bool> SaveSettings(SettingsStorageModel settings, StorageConnection connection = null)
        {
            bool result = false;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM SETTING WHERE SettingName='{0}'", settings.SettingName);
                    SettingsStorageModel settingToUpdate = null;
                    using (var dbDataReader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (dbDataReader.HasRows && dbDataReader.Read())
                        {
                            settingToUpdate = new SettingsStorageModel()
                            {
                                SettingId = dbDataReader.GetInt64(0),
                                ProfileId = dbDataReader.GetInt64(1),
                                SettingName = dbDataReader.GetString(2),
                                SettingValue = settings.SettingValue
                            };
                        }
                        dbDataReader.Close();
                    }
                    if (settingToUpdate != null)
	                {
                        command.CommandText = string.Format(
                            "UPDATE SETTING SET ProfileId = {0}, SettingName = '{1}', SettingValue = '{2}' WHERE SettingId = {3}",
                            settingToUpdate.ProfileId, settingToUpdate.SettingName, settingToUpdate.SettingValue, settingToUpdate.SettingId);
                    }
                    else
                    {
                        command.CommandText = string.Format("INSERT INTO SETTING(ProfileId, SettingName, SettingValue) VALUES ({0}, '{1}', '{2}')",
                            settings.ProfileId, settings.SettingName, settings.SettingValue);
                    }
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
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
                    command.CommandText = string.Format("SELECT * FROM SETTING");
                    DbDataReader dbDataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);

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
        public async Task<bool> SavePurchaseItem(PurchaseItemModel purchaseItem)
        {
            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    if (purchaseItem.PurchaseItemId == 0)
                    {
                        a store name, rating etc??
                        command.CommandText = string.Format(
                        "INSERT INTO PURCHASEITEM(PurchaseId, Timestamp, TotalCost, ItemCost, ItemsNumber, StoreId) VALUES ('{0}', {1}, {2}, {3}, {4}, {5})",
                        purchaseItem.PurchaseId, purchaseItem.Timestamp, purchaseItem.TotalCost.ToString(formatCulture), purchaseItem.ItemCost.ToString(formatCulture), purchaseItem.ItemsNumber.ToString(formatCulture), purchaseItem.StoreId);
                    }
                    else
                    {
                        command.CommandText = string.Format(
                        "UPDATE PURCHASE SET PurchaseId = '{0}', Timestamp = {1}, TotalCost = {2}, ItemCost = {3}, ItemsNumber = {4}, StoreId = {5} WHERE PurchaseItemId = {6}",
                        purchaseItem.PurchaseId, purchaseItem.Timestamp, purchaseItem.TotalCost.ToString(formatCulture), purchaseItem.ItemCost.ToString(formatCulture), purchaseItem.ItemsNumber.ToString(formatCulture), purchaseItem.StoreId, purchaseItem.PurchaseItemId);
                    }

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"SavePurchaseItem\": {0}", ex.Message);
            }
            return result;
        }
        public async Task<bool> SavePurchase(PurchaseModel purchase)
        {
            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    if (purchase.PurchaseId == 0)
                    {
                        command.CommandText = string.Format(
                        "INSERT INTO PURCHASE(Name, TypeId, SubTypeId, IsMonthly) VALUES ('{0}', {1}, {2}, {3})",
                        purchase.Name, purchase.TypeId, purchase.SubTypeId, purchase.IsMonthly);
                    }
                    else
                    {
                        command.CommandText = string.Format(
                        "UPDATE PURCHASE SET Name = '{0}', TypeId = {1}, SubTypeId = {2}, IsMonthly = {3} WHERE PurchaseId = {4}",
                        purchase.Name, purchase.TypeId, purchase.SubTypeId, purchase.IsMonthly, purchase.PurchaseId);
                    }

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"SavePurchase\": {0}", ex.Message);
            }
            return result;
        }
        public async Task<bool> SavePurchaseItemBulk(IEnumerable<PurchaseItemModel> purchases, StorageConnection connection = null)
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
                        "INSERT INTO PURCHASE(PurchaseId, Timestamp, TotalCost, ItemCost, ItemNumber, StoreId) VALUES ('{0}', {1}, {2}, {3}, {4}, {5})",
                        purchase.PurchaseId, purchase.Timestamp, purchase.TotalCost.ToString(formatCulture), purchase.ItemCost.ToString(formatCulture), purchase.ItemsNumber.ToString(formatCulture), purchase.StoreId);

                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"SavePurchaseItemBulk\": {0}", ex.Message);
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
                        command.CommandText = string.Format("INSERT INTO PURCHASETYPE (Name) VALUES ('{0}')", purchaseType.Name);
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE PURCHASETYPE SET Name = '{0}' WHERE TypeId = {1}", purchaseType.Name, purchaseType.TypeId);
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
        public async Task<bool> SavePurchaseSubType(PurchaseSubTypeModel purchaseSubType, StorageConnection connection = null)
        {

            bool result = false;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    if (purchaseSubType.Id == 0)
                    {
                        command.CommandText = string.Format("INSERT INTO PURCHASESUBTYPE (Name) VALUES ('{0}')", purchaseSubType.Name);
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE PURCHASESUBTYPE SET Name = '{0}' WHERE Id = {1}", purchaseSubType.Name, purchaseSubType.Id);
                    }
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"SavePurchaseSubType\": {0}", ex.Message);
            }
            return result;
        }
        public async Task<PurchaseItemModel> LoadPurchaseItem(int id)
        {
            PurchaseItemModel purchaseItem = null;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM PURCHASEITEM WHERE PurchaseItemId = {0}", id);
                    var dbReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    if (dbReader.HasRows && dbReader.Read())
                    {
                        purchaseItem = new PurchaseItemModel
                        {
                            PurchaseId = dbReader.GetInt64(0),
                            Timestamp = dbReader.GetInt64(1),
                            TotalCost = dbReader.GetDouble(2),
                            ItemCost = dbReader.GetDouble(3),
                            ItemsNumber = dbReader.GetDouble(4),
                            StoreId = dbReader.GetInt32(5)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadPurchaseItem\": {0}", ex.Message);
            }
            return purchaseItem;
        }
        public async Task<PurchaseModel> LoadPurchase(int id, StorageConnection connection = null)
        {
            PurchaseModel purchase = null;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM PURCHASE WHERE PurchaseId = {0}", id);
                    var dbReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    if (dbReader.HasRows && dbReader.Read())
                    {
                        purchase = new PurchaseModel
                        {
                            PurchaseId = dbReader.GetInt64(0),
                            Name = dbReader.GetString(1),
                            TypeId = dbReader.GetInt32(2),
                            SubTypeId =dbReader.GetInt32(3),
                            IsMonthly = dbReader.GetBoolean(4)
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
        public async Task<List<PurchaseModel>> LoadPurchaseList()
        {
            var list = new List<PurchaseModel>();

            try
            {
                logger.Debug("DatabaseService.LoadPurchaseList: Loading purchase list");

                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM PURCHASE";

                    var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    
                    while (dataReader.Read())
                    {
                        list.Add(new PurchaseModel
                        {
                            PurchaseId = dataReader.GetInt64(0),
                            Name = dataReader.GetString(1),
                            TypeId = dataReader.GetInt32(2),
                            SubTypeId = dataReader.GetInt32(3),
                            IsMonthly = dataReader.GetBoolean(4)
                        });
                    }

                    logger.Debug("DatabaseService.LoadPurchaseList: data fetched succesfully");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadPurchaseList\": {0}", ex.Message);
            }
            return list;
        }
        public async Task<List<PurchaseItemModel>> LoadPurchaseItemList(SearchRequestModel filter)
        {
            var list = new List<PurchaseItemModel>();

            try
            {
                logger.Debug("DatabaseService.LoadPurchaseItemList: Loading purchase items list");

                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    string queue = "SELECT P.PurchaseId, Name, Timestamp, TotalCost, ItemCost, ItemsNumber, TypeId, SubTypeId, StoreId, IsMonthly FROM PURCHASEITEM PI JOIN PURCHASE P ON PI.PurchaseId=P.PurchaseId WHERE";
                    a store name, rating etc??
                    if (filter.SearchByName)
                    {
                        queue = string.Format("{0} Name LIKE '%{1}%' ", queue, filter.Name.Trim(' '));
                    }
                    if (filter.SearchByType)
                    {
                        queue = string.Format("{0} TypeId = {1} ", queue, filter.TypeId);
                    }
                    if (filter.SearchBySubType)
                    {
                        queue = string.Format("{0} SubTypeId = {1} ", queue, filter.TypeId);
                    }
                    if (filter.SearchByDate)
                    {
                        queue = string.Format("{0} Timestamp BETWEEN {1} AND {2} ", queue, filter.DateStart.Ticks, filter.DateEnd.Ticks);
                    }
                    if (filter.SearchByCost)
                    {
                        queue = string.Format("{0} TotalCost BETWEEN {1} AND {2} ", queue, filter.CostStart, filter.CostEnd);
                    }
                    if (filter.SearchByMonthly)
                    {
                        queue = string.Format("{0} IsMonthly = {1} ", queue, filter.IsMonthly);
                    }

                    command.CommandText = queue
                        .TrimEnd(" WHERE")
                        .TrimEnd(' ')
                        .Replace("  ", " AND ");

                    logger.Debug("DatabaseService.LoadPurchaseItemList: queue: {0}", command.CommandText);

                    var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                    while (dataReader.Read())
                    {
                        list.Add(new PurchaseItemModel
                        {
                            PurchaseId = dataReader.GetInt64(0),
                            Name = dataReader.GetString(1),
                            Timestamp = dataReader.GetInt64(2),
                            TotalCost = dataReader.GetDouble(3),
                            ItemCost = dataReader.GetDouble(4),
                            ItemsNumber = dataReader.GetDouble(5),
                            TypeId = dataReader.GetInt32(6),
                            SubTypeId = dataReader.GetInt32(7),
                            StoreId = dataReader.GetInt32(8),
                            IsMonthly = dataReader.GetBoolean(9)
                        });
                    }

                    logger.Debug("DatabaseService.LoadPurchaseList: data fetched succesfully");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadPurchaseList\": {0}", ex.Message);
            }
            return list;
        }
        public async Task<List<PurchaseItemModel>> LoadCompletePurchaseItemList()
        {
            logger.Debug("DatabaseService: Loading complete purchase items list");
            return await LoadPurchaseItemList(new SearchRequestModel());
        }
        public async Task<IEnumerable<PurchaseTypeModel>> LoadPurchaseTypeList(StorageConnection connection = null)
        {
            var list = new List<PurchaseTypeModel>();
            try
            {
                using(var db = connection ?? dbManager.GetConnection())
                using(var command = db.Connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM PURCHASETYPE";
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

        public async Task<IEnumerable<PurchaseSubTypeModel>> LoadPurchaseSubTypeList(StorageConnection connection = null)
        {
            var list = new List<PurchaseSubTypeModel>();
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM PURCHASESUBTYPE";
                    var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    while (dataReader.Read())
                    {
                        list.Add(new PurchaseSubTypeModel
                        {
                            Id = dataReader.GetInt64(0),
                            Name = dataReader.GetString(1)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadPurchaseSubTypeList\": {0}", ex.Message);
            }
            return list;
        }

        public async Task<bool> RemovePurchase(long purchaseId, StorageConnection connection = null)
        {
            bool result = false;
            try
            {
                using(var db = connection ?? dbManager.GetConnection())
                using(var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM PURCHASE WHERE PurchaseId = {0}", purchaseId);
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
        public async Task<bool> DeletePurchaseItem(long purchaseItemId)
        {
            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM PURCHASEITEM WHERE PurchaseItemId = {0}", purchaseItemId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"DeletePurchaseItem\": {0}", ex.Message);
            }

            return result;
        }

        public async Task<bool> DeletePurchaseType(PurchaseTypeModel type, StorageConnection connection = null)
        {
            bool result = false;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM PURCHASETYPE WHERE TypeId = {0}", type.TypeId);
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
        public async Task<bool> DeletePurchaseSubType(PurchaseSubTypeModel subType, StorageConnection connection = null)
        {
            bool result = false;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM PURCHASESUBTYPE WHERE Id = {0}", subType.Id);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"DeletePurchaseSubType\": {0}", ex.Message);
            }

            return result;
        }
    }
}

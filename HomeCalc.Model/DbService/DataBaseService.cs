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
                    case "PURCHASETYPEMODELS":
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
                using (var command = connection.Connection.CreateCommand())
                {
                    foreach (var table in DefaultDbContent.Tables.Keys)
                    {
                        if (!IsTableExists(table))
                        {
                            command.CommandText = string.Format("create table {0} {1}", table, DefaultDbContent.Tables[table]);
                            command.ExecuteNonQuery();
                        }
                    }
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
                    command.CommandText = string.Format("SELECT * FROM SETTINGMODELS WHERE SettingName='{0}'", settings.SettingName);
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
                            "UPDATE SETTINGMODELS SET ProfileId = {0}, SettingName = '{1}', SettingValue = '{2}' WHERE SettingId = {3}",
                            settingToUpdate.ProfileId, settingToUpdate.SettingName, settingToUpdate.SettingValue, settingToUpdate.SettingId);
                    }
                    else
                    {
                        command.CommandText = string.Format("INSERT INTO SETTINGMODELS(ProfileId, SettingName, SettingValue) VALUES ({0}, '{1}', '{2}')",
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
                    command.CommandText = string.Format("SELECT * FROM SETTINGMODELS");
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
                        "INSERT INTO PURCHASEMODELS(Name, Timestamp, TotalCost, ItemCost, ItemsNumber, TypeId) VALUES ('{0}', {1}, {2}, {3}, {4}, {5})",
                        purchase.Name, purchase.Timestamp, purchase.TotalCost.ToString(formatCulture), purchase.ItemCost.ToString(formatCulture), purchase.ItemsNumber.ToString(formatCulture), purchase.TypeId);
                    }
                    else
                    {
                        command.CommandText = string.Format(
                        "UPDATE PURCHASEMODELS SET Name = '{0}', Timestamp = {1}, TotalCost = {2}, ItemCost = {3}, ItemsNumber = {4}, TypeId = {5} WHERE PurchaseId = {6}",
                        purchase.Name, purchase.Timestamp, purchase.TotalCost.ToString(formatCulture), purchase.ItemCost.ToString(formatCulture), purchase.ItemsNumber.ToString(formatCulture), purchase.TypeId, purchase.PurchaseId);
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
                        "INSERT INTO PURCHASEMODELS(Name, Timestamp, TotalCost, ItemCost, ItemNumber, TypeId) VALUES ('{0}', {1}, {2}, {3}, {4}, {5})",
                        purchase.Name, purchase.Timestamp, purchase.TotalCost.ToString(formatCulture), purchase.ItemCost.ToString(formatCulture), purchase.ItemsNumber.ToString(formatCulture), purchase.TypeId);

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
                        command.CommandText = string.Format("INSERT INTO PURCHASETYPEMODELS (Name) VALUES ('{0}')", purchaseType.Name);
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE PURCHASETYPEMODELS SET Name = '{0}' WHERE TypeId = {1}", purchaseType.Name, purchaseType.TypeId);
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
            {
                logger.Debug("DatabaseService.LoadPurchaseList: Loading purchase list");

                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    string queue = "SELECT * FROM PURCHASEMODELS WHERE";
                    if (filter.SearchByName)
                    {
                        queue = string.Format("{0} Name LIKE '%{1}%' ", queue, filter.Name.Trim(' '));
                    }
                    if (filter.SearchByType)
                    {
                        queue = string.Format("{0} TypeId = {1} ", queue, filter.TypeId);
                    }
                    if (filter.SearchByDate)
                    {
                        queue = string.Format("{0} Timestamp BETWEEN {1} AND {2} ", queue, filter.DateStart.Ticks, filter.DateEnd.Ticks);
                    }
                    if (filter.SearchByCost)
                    {
                        queue = string.Format("{0} TotalCost BETWEEN {1} AND {2} ", queue, filter.CostStart, filter.CostEnd);
                    }

                    command.CommandText = queue
                        .TrimEnd(" WHERE")
                        .TrimEnd(' ')
                        .Replace("  ", " AND ");

                    logger.Debug("DatabaseService.LoadPurchaseList: queue: {0}", command.CommandText);

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

                    logger.Debug("DatabaseService.LoadPurchaseList: data fetched succesfully");
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
            logger.Debug("DatabaseService: Loading complete purchase list");
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
                    command.CommandText = string.Format("DELETE FROM PURCHASEMODELS WHERE PurchaseId = {0}", purchaseId);
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
        public async Task<bool> DeletePurchaseSubType(PurchaseSubTypeModel subType, StorageConnection connection = null)
        {
            bool result = false;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM PURCHASESUBTYOE WHERE Id = {0}", subType.Id);
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

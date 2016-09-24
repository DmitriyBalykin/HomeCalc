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
    public partial class DataBaseService
    {
        public async Task<long> SaveStore(StoreModel store)
        {
            long storeId = -1;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    if (store.Id == 0)
                    {
                        command.CommandText = string.Format("SELECT Id FROM STORE WHERE Name='{0}'", store.Name);
                        storeId = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0L);
                    }
                    else
                    {
                        storeId = store.Id;
                    }
                    if (storeId == 0)
                    {
                        command.CommandText = string.Format("INSERT INTO STORE(Name) VALUES ('{0}'); SELECT last_insert_rowid() FROM STORE", store.Name);
                        storeId = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false));
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE STORE SET Name = '{0}' WHERE Id = {1}", store.Name, storeId);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"SaveStore\": {0}", ex.Message);
            }
            return storeId;
        }
        public async Task<StoreModel> LoadStore(long id)
        {
            StoreModel store = null;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM STORE WHERE Id = {0}", id);
                    var dbReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    if (dbReader.HasRows && dbReader.Read())
                    {
                        store = new StoreModel
                        {
                            Id = dbReader.GetInt64(0),
                            Name = dbReader.GetString(1)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadStore\": {0}", ex.Message);
            }
            return store;
        }
        public async Task<bool> DeleteStore(long storeId)
        {
            bool result = false;
            try
            {
                using(var db = dbManager.GetConnection())
                using(var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM STORE WHERE Id = {0}", storeId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"DeleteStore\": {0}", ex.Message);
            }
            
            return result;
        }
    }
}

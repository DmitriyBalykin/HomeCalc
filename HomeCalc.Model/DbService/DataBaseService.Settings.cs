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
    }
}

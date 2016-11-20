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
using System.Threading;

namespace HomeCalc.Model.DbService
{
    public partial class DataBaseService
    {
        public async Task<bool> SaveSettings(SettingsStorageModel settings)
        {
            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    Console.WriteLine("selecting settings, thread {0}", Thread.CurrentThread.ManagedThreadId);
                    command.CommandText = string.Format("SELECT * FROM SETTING WHERE SettingName='{0}'", settings.SettingName);
                    SettingsStorageModel settingToUpdate = null;
                    Console.WriteLine("selecting settings end, thread {0}", Thread.CurrentThread.ManagedThreadId);
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
                        Console.WriteLine("updating settings, thread {0}", Thread.CurrentThread.ManagedThreadId);
                        command.CommandText = string.Format(
                            "UPDATE SETTING SET ProfileId = {0}, SettingName = '{1}', SettingValue = '{2}' WHERE SettingId = {3}",
                            settingToUpdate.ProfileId, settingToUpdate.SettingName, settingToUpdate.SettingValue, settingToUpdate.SettingId);
                    }
                    else
                    {
                        Console.WriteLine("inserting settings, thread {0}", Thread.CurrentThread.ManagedThreadId);
                        command.CommandText = string.Format("INSERT INTO SETTING(ProfileId, SettingName, SettingValue) VALUES ({0}, '{1}', '{2}')",
                            settings.ProfileId, settings.SettingName, settings.SettingValue);
                    }
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    Console.WriteLine("update/insert settings end settings, thread {0}", Thread.CurrentThread.ManagedThreadId);
                }
                result = true;
                Console.WriteLine("settings db connection closed, thread {0}", Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("settings db connection exception, thread {0}, text: {0}", Thread.CurrentThread.ManagedThreadId, ex.Message);
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

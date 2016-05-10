using HomeCalc.Model.DbConnectionWrappers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DbService
{
    class SQLiteManager : IDatabaseManager
    {
        private static string dbFilePath = "DataStorage.sqlite";
        private static string containerFolder = "HomeCalc";

        public StorageConnection GetConnection()
        {
            
#if DEBUG
        dbFilePath = "DataStorage_Debug.sqlite";
#endif
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var containerFolderPath = Path.Combine(appDataFolder, containerFolder);
            if (!Directory.Exists(containerFolderPath))
            {
                Directory.CreateDirectory(containerFolderPath);
            }
            string fullPath = Path.Combine(containerFolderPath, dbFilePath);
            string connString = "Data Source=" + fullPath;
            lock (this)
            {
                if (!File.Exists(fullPath))
                {
                    SQLiteConnection.CreateFile(fullPath);
                }
                StorageConnection connection = new StorageConnection(connString);
                if (connection != null && (connection.State == System.Data.ConnectionState.Connecting || connection.State == System.Data.ConnectionState.Open))
                {
                    InitializeDbScheme(connection);
                    InitializeDbContent(connection);
                    return connection;
                }
                else
                {
                    throw new Exception(string.Format("Database is not available with connection string {0}", connString));
                }
            }
            
        }

        private void InitializeDbContent(StorageConnection connection)
        {
            var dbService = DataBaseService.GetInstance();
            if (!connection.RecentlyInitiated)
            {
                return;
            }
            foreach (var value in DefaultDbContent.Values)
            {
                switch (value.Table)
                {
                    case "PURCHASETYPEMODELS":
                        var valueExist = dbService.LoadPurchaseTypeList(connection).Result.Any(p => p.Name == value.Value.Name);
                        if (!valueExist)
                        {
                            var result = dbService.SavePurchaseType(new DataModels.PurchaseTypeModel { Name = value.Value.Name }, connection).Result;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void InitializeDbScheme(StorageConnection connection)
        {
            foreach (var table in DefaultDbContent.Tables.Keys)
            {
                if (!IsTableExists(connection.Connection, table))
                {
                    connection.RecentlyInitiated = true;
                    new SQLiteCommand(string.Format("create table {0} {1}", table, DefaultDbContent.Tables[table]), connection.Connection).ExecuteNonQuery();
                }
            }
        }

        private bool IsTableExists(SQLiteConnection conn, string table)
        {
            try
            {
                new SQLiteCommand(string.Format("SELECT * FROM {0}", table), conn).ExecuteNonQuery();
                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }
    }
}

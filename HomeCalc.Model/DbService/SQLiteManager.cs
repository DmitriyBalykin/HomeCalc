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
using System.Threading;
using System.Threading.Tasks;

namespace HomeCalc.Model.DbService
{
    class SQLiteManager : IDatabaseManager
    {
        private static string dbFilePath = "DataStorage.sqlite";
        private static string containerFolder = "HomeCalc";
        private object monitor = new object();

        public StorageConnection GetConnection()
        {
            string fullDbFilePath;
            string connString = GetConnectionString(out fullDbFilePath);

            lock (monitor)
            {
                if (!File.Exists(fullDbFilePath))
                {
                    SQLiteConnection.CreateFile(fullDbFilePath);
                    InitializeDbScheme();
                    InitializeDbContent();
                }
            }

            StorageConnection connection = CreateConnection(connString);
            if (connection != null)
            {
                return connection;
            }
            else
            {
                throw new Exception(string.Format("Database is not available with connection string {0}", connString));
            }
        }
        private StorageConnection GetConnectionUnsafe()
        {
            var connString = GetConnectionString();
            StorageConnection connection = CreateConnection(connString);
            if (connection != null)
            {
                return connection;
            }
            else
            {
                throw new Exception(string.Format("Database is not available with connection string {0}", connString));
            }
        }

        private StorageConnection CreateConnection(string connectionString)
        {
            StorageConnection connection = new StorageConnection(connectionString);
            if (connection != null && (connection.State == System.Data.ConnectionState.Connecting || connection.State == System.Data.ConnectionState.Open))
            {
                return connection;
            }
            else
            {
                return null;
            }
        }

        private string GetConnectionString(out string fullDbFilePath)
        {
#if DEBUG
            dbFilePath = "DataStorage_Debug.sqlite";
#endif
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var containerFolderPath = Path.Combine(appDataFolder, containerFolder);

            fullDbFilePath = Path.Combine(containerFolderPath, dbFilePath);
            string connString = "Data Source=" + fullDbFilePath;

            return connString;
        }
        private string GetConnectionString()
        {
            string fullDbFilePath;
            return GetConnectionString(out fullDbFilePath);
        }

        private void InitializeDbContent()
        {
            var dbService = DataBaseService.GetInstance();
            foreach (var value in DefaultDbContent.Values)
            {
                switch (value.Table)
                {
                    case "PURCHASETYPEMODELS":
                        var valueExist = dbService.LoadPurchaseTypeList(GetConnectionUnsafe()).Result.Any(p => p.Name == value.Value.Name);
                        if (!valueExist)
                        {
                            var result = dbService.SavePurchaseType(new DataModels.PurchaseTypeModel { Name = value.Value.Name }, GetConnectionUnsafe()).Result;
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
                using (var connection = GetConnectionUnsafe())
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
                using (var connection = GetConnectionUnsafe())
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
    }
}

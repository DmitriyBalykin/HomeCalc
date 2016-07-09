using HomeCalc.Core.LogService;
using HomeCalc.Core.Services;
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
    class SQLiteManager: IDatabaseManager
    {
        private object monitor = new object();
        private static object instanceMonitor = new object();

        private bool storageInitiated = false;

        private Logger logger = LogService.GetLogger();
        private StatusService statusService = StatusService.GetInstance();

        private SQLiteManager()
        {
            InitializeStorage();
        }

        private static IDatabaseManager instance;
        public static IDatabaseManager GetInstance()
        {
            lock (instanceMonitor)
            {
                if (instance == null)
                {
                    instance = new SQLiteManager();
                }
            }
            return instance;
        }
        private void InitializeStorage()
        {
            string fullDbFilePath = FilenameService.GetDBPath();
            string connString = GetConnectionString();
            lock (monitor)
            {
                try
                {
                    if (!File.Exists(fullDbFilePath))
                    {
                        SQLiteConnection.CreateFile(fullDbFilePath);
                    }

                    storageInitiated = true;
                }
                catch (Exception)
                {}
            }
        }
        public StorageConnection GetConnection(bool skipInitiatedCheck = false)
        {
            if (!skipInitiatedCheck && !storageInitiated)
            {
                logger.Error("Cannot created connection to database: storage not initialized");
                statusService.Post("Помилка: база даний не ініційована, продовження роботи неможливе");
            }
            string fullDbFilePath = FilenameService.GetDBPath();
            string connString = GetConnectionString();

            StorageConnection connection = new StorageConnection(connString);
            if (connection != null && (connection.State == System.Data.ConnectionState.Connecting || connection.State == System.Data.ConnectionState.Open))
            {
                return connection;
            }
            else
            {
                throw new Exception(string.Format("Database is not available with connection string {0}", connString));
            }
        }

        private string GetConnectionString()
        {
            return "Data Source=" + FilenameService.GetDBPath();
        }

        
    }
}

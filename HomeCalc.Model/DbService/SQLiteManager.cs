using HomeCalc.Model.DbConnectionWrappers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
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
        private StorageConnection GetConnection()
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
                    InitializeDb(connection);
                    return connection;
                }
                else
                {
                    throw new Exception(string.Format("Database is not available with connection string {0}", connString));
                }
            }
            
        }
        public StorageContext GetContext()
        {
            try
            {
                StorageConnection connection = GetConnection();
                StorageContext context = new StorageContext(connection.Connection);
                if (connection.RecentlyInitiated)
                {
                    InitializeContext(context);    
                }
                return context;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void InitializeContext(StorageContext context)
        {
            foreach (var value in DefaultDbContent.Values)
            {
                switch (value.Table)
                {
                    case "PURCHASETYPEMODELS":
                        bool dbInitiated = false;
                        try
                        {
                            if (context.PurchaseType.Where(p => p.Name == value.Value.Name).Count() != 0)
	                        {
		                        dbInitiated = true;
	                        }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        if (!dbInitiated)
                        {
                            context.PurchaseType.Add(new DataModels.PurchaseTypeModel { Name = value.Value.Name });
                            context.SaveChanges();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void InitializeDb(StorageConnection connection)
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

    class SQLiteDbConfiguration : DbConfiguration
    {
        public SQLiteDbConfiguration()
        {
            string assemblyName = typeof(SQLiteProviderFactory).Assembly.GetName().Name;
            RegisterDbProviderFactories(assemblyName);
            SetProviderFactory(assemblyName, SQLiteFactory.Instance);
            SetProviderFactory(assemblyName, SQLiteProviderFactory.Instance);
            SetProviderServices(assemblyName, (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof (DbProviderServices)));
        }

        private void RegisterDbProviderFactories(string assemblyName)
        {
            var dataSet = ConfigurationManager.GetSection("system.data") as DataSet;
            if (dataSet != null)
            {
                var dbProviderFactoriesDatatable = dataSet.Tables.OfType<DataTable>().FirstOrDefault(x => x.TableName == typeof(DbProviderFactories).Name);
                var dataRow = dbProviderFactoriesDatatable.Rows.OfType<DataRow>().FirstOrDefault(x => x.ItemArray[2].ToString() == assemblyName);
                if (dataRow != null)
                {
                    dbProviderFactoriesDatatable.Rows.Remove(dataRow);
                }
                dbProviderFactoriesDatatable.Rows.Add("SQLite Data Provider (Entity Framework 6)", ".NET Framework Data Provider for SQLite (Entity Framework 6)", assemblyName, typeof(SQLiteProviderFactory).Name);
            }
        }
    }
}

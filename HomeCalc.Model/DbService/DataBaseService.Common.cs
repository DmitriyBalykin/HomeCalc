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

        private void InitializeDbContent()
        {
            foreach (var value in DefaultDbContent.Values)
            {
                switch (value.Table)
                {
                    case "PRODUCTTYPE":
                        var valueExist = LoadProductTypeList().Result.Any(p => p.Name == value.Value.Name);
                        if (!valueExist)
                        {
                            var result = SaveProductType(new DataModels.ProductTypeModel { Name = value.Value.Name }).Result;
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
                        case 1:
                            //create common tables
                            command.CommandText =
                                    "create table if not exists PRODUCTSUBTYPE (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, TypeId INTEGER);" +
                                    "create table if not exists STORE (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT);" +
                                    "create table if not exists PRODUCT (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, TypeId INTEGER, SubTypeId INTEGER, IsMonthly BOOLEAN);" +
                                    "create table if not exists PURCHASE (Id INTEGER PRIMARY KEY AUTOINCREMENT, ProductId INTEGER, Timestamp INTEGER, TotalCost REAL, ItemCost REAL, ItemsNumber REAL, StoreId INTEGER);" +
                                    "create table if not exists COMMENT (Id INTEGER PRIMARY KEY AUTOINCREMENT, PurchaseId INTEGER, StoreId INTEGER, Text TEXT, Rate INTEGER);";
                                command.ExecuteNonQuery();
                            if (IsTableExists("PURCHASETYPEMODELS") && IsTableExists("SETTINGMODELS") && IsTableExists("PURCHASEMODELS"))
                            {
                                //alter existed tables
                                command.CommandText =
                                    "ALTER TABLE PURCHASETYPEMODELS RENAME TO PRODUCTTYPE;" +
                                    "ALTER TABLE SETTINGMODELS RENAME TO SETTING;"+
                                    "INSERT INTO PRODUCT (Name, TypeId) SELECT DISTINCT Name, TypeId FROM PURCHASEMODELS group by Name;" +
                                    "INSERT INTO PURCHASE (Id, ProductId, Timestamp, TotalCost, ItemCost, ItemsNumber) SELECT pm.PurchaseId, p.Id, Timestamp, TotalCost, ItemCost, ItemsNumber FROM PURCHASEMODELS pm JOIN PRODUCT p on pm.Name=p.Name;"+
                                    "DROP TABLE PURCHASEMODELS";
                                    command.ExecuteNonQuery();
                            }
                            else
                            {
                                //else create new tables
                                command.CommandText =
                                    "create table if not exists PRODUCTTYPE (TypeId INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT);"+
                                    "create table if not exists SETTING (SettingId INTEGER PRIMARY KEY AUTOINCREMENT, ProfileId INTEGER, SettingName TEXT, SettingValue TEXT);";
                                command.ExecuteNonQuery();
                            }

                            command.CommandText = "PRAGMA user_version=2;";
                            command.ExecuteNonQuery();
                            break;
                        default:
                            return;
                    }
                    transaction.Commit();
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database initialization failed, "+ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Unrecognized db exception: {0}", ex.Message);
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
            catch (SQLiteException ex)
            {
                Console.WriteLine("Table {0} not exist", table);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unrecognized db exception: {0}", ex.Message);
            }
            return false;
        }      
    }
}

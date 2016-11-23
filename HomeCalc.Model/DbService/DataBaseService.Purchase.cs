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
        public async Task<long> SavePurchase(PurchaseModel purchase)
        {
            long purchaseId = purchase.Id;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var transaction = db.Connection.BeginTransaction())
                using (var command = db.Connection.CreateCommand())
                {
                    if (purchaseId == 0)
                    {
                        command.CommandText = string.Format(
                        "INSERT INTO PURCHASE(ProductId, Timestamp, TotalCost, ItemCost, ItemsNumber, StoreId) VALUES ({0}, {1}, {2}, {3}, {4}, {5}); SELECT last_insert_rowid() FROM PURCHASE",
                        purchase.ProductId,
                        purchase.Timestamp,
                        purchase.TotalCost.ToString(formatCulture),
                        purchase.ItemCost.ToString(formatCulture),
                        purchase.ItemsNumber.ToString(formatCulture),
                        purchase.StoreId);
                        purchaseId = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false));
                        logger.Debug("Saving new purchase with name={0} and id={1}", purchase.ProductName, purchaseId);
                    }
                    else
                    {
                        command.CommandText = string.Format(
                        "UPDATE PURCHASE SET ProductId = {0}, Timestamp = {1}, TotalCost = {2}, ItemCost = {3}, ItemsNumber = {4}, StoreId = {5} WHERE Id = {6}",
                        purchase.ProductId,
                        purchase.Timestamp,
                        purchase.TotalCost.ToString(formatCulture),
                        purchase.ItemCost.ToString(formatCulture),
                        purchase.ItemsNumber.ToString(formatCulture),
                        purchase.StoreId,
                        purchase.Id);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        logger.Debug("Updating existed purchase with name={0} and id={1}", purchase.ProductName, purchaseId);
                    }
                    
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                purchaseId = -1;
                logger.Error("Exception during execution method \"SavePurchase\": {0}", ex.Message);
            }
            return purchaseId;
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
                        "INSERT INTO PURCHASE(PurchaseId, Timestamp, TotalCost, ItemCost, ItemNumber, StoreId) VALUES ('{0}', {1}, {2}, {3}, {4}, {5})",
                        purchase.Id, purchase.Timestamp, purchase.TotalCost.ToString(formatCulture), purchase.ItemCost.ToString(formatCulture), purchase.ItemsNumber.ToString(formatCulture), purchase.StoreId);

                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        logger.Debug("Saving new purchase with name={0} and id={1}", purchase.ProductName, purchase.Id);
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
        public async Task<PurchaseModel> LoadPurchase(long id)
        {
            PurchaseModel purchase = null;
            try
            {
                purchase = (await LoadPurchaseList(new SearchRequestModel { SearchById = true, PurchaseId = id }).ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadPurchase\": {0}", ex.Message);
            }
            return purchase;
        }
        public async Task<List<PurchaseModel>> LoadPurchaseList(SearchRequestModel filter)
        {
            var list = new List<PurchaseModel>();

            try
            {
                logger.Debug("DatabaseService.LoadPurchaseList: Loading purchase items list");

                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    string queue = "SELECT pu.Id AS PurchaseId, pu.Timestamp, pu.TotalCost, pu.ItemCost, pu.ItemsNumber," +
                                    "p.Id AS ProductId, p.Name AS ProductName, p.TypeId, p.SubTypeId, p.IsMonthly, "+
                                    "s.Id AS StoreId, s.Name AS StoreName, "+
                                    "c.Text AS Comment, c.Rate "+
                    "FROM PURCHASE pu "+
                    "LEFT JOIN PRODUCT p ON pu.ProductId=p.Id "+
                    "LEFT JOIN STORE s ON pu.StoreId=s.Id "+
                    "LEFT JOIN COMMENT c ON pu.Id=c.PurchaseId "+
                    "WHERE";
                    if (filter.SearchById)
                    {
                        queue = string.Format("{0} PurchaseId={1} ", queue, filter.PurchaseId);
                    }
                    if (filter.SearchByName)
                    {
                        queue = string.Format("{0} ProductName LIKE '%{1}%' ", queue, StringUtilities.EscapeStringForDatabase(filter.Name.Trim(' ')));
                    }
                    if (filter.SearchByType)
                    {
                        queue = string.Format("{0} TypeId={1} ", queue, filter.TypeId);
                    }
                    if (filter.SearchByDate)
                    {
                        queue = string.Format("{0} Timestamp BETWEEN {1} AND {2} ", queue, filter.DateStart.Ticks, filter.DateEnd.Ticks);
                    }
                    if (filter.SearchByCost)
                    {
                        queue = string.Format("{0} TotalCost BETWEEN {1} AND {2} ", queue, filter.CostStart, filter.CostEnd);
                    }
                    if (filter.SearchByCommentId)
                    {
                        queue = string.Format("{0} CommentID={1} ", queue, filter.CommentId);
                    }
                    if (filter.SearchByStoreId)
                    {
                        queue = string.Format("{0} StoreID={1} ", queue, filter.StoreId);
                    }
                    if (filter.SearchByMonthly)
                    {
                        var isMonthlyString = string.Format(" (IsMonthly='{0}' ", filter.IsMonthly);
                        if (filter.IsMonthly)
                        {
                            isMonthlyString += ")";
                        }
                        else
                        {
                            isMonthlyString += "OR IsMonthly is NULL) ";
                        }

                        queue = string.Format("{0} {1} ", queue, isMonthlyString);
                        
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
                            Id = dataReader.GetInt64(dataReader.GetOrdinal("PurchaseId")),
                            ProductName = dataReader.GetString(dataReader.GetOrdinal("ProductName")),
                            ProductId = dataReader.GetInt64(dataReader.GetOrdinal("ProductId")),
                            Timestamp = dataReader.GetInt64(dataReader.GetOrdinal("Timestamp")),
                            TotalCost = dataReader.GetDouble(dataReader.GetOrdinal("TotalCost")),
                            ItemCost = dataReader.GetDouble(dataReader.GetOrdinal("ItemCost")),
                            ItemsNumber = dataReader.GetDouble(dataReader.GetOrdinal("ItemsNumber")),
                            TypeId = dataReader.GetInt64(dataReader.GetOrdinal("TypeId")),
                            SubTypeId = (dataReader.GetValue(dataReader.GetOrdinal("SubTypeId")) as long?) ?? 0L,
                            Rate = (int)((dataReader.GetValue(dataReader.GetOrdinal("Rate")) as long?) ?? 0L),
                            Comment = (dataReader.GetValue(dataReader.GetOrdinal("Comment")) as string) ?? string.Empty,
                            IsMonthly = (dataReader.GetValue(dataReader.GetOrdinal("IsMonthly")) as bool?) ?? false,
                            StoreId = (dataReader.GetValue(dataReader.GetOrdinal("StoreId")) as long?) ?? 0L,
                            StoreName = (dataReader.GetValue(dataReader.GetOrdinal("StoreName")) as string) ?? string.Empty
                        });
                    }

                    logger.Debug("DatabaseService.LoadPurchaseList: data fetched succesfully");
                    logger.Debug("Found {0} items", list.Count);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadPurchaseList\": {0}", ex.Message);
            }
            return list;
        }
        public async Task<List<PurchaseModel>> LoadCompletePurchaseList()
        {
            logger.Debug("DatabaseService: Loading complete purchase list");
            return await LoadPurchaseList(new SearchRequestModel());
        }
        public async Task<bool> DeletePurchase(long purchaseId)
        {
            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var transaction = db.Connection.BeginTransaction())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM PURCHASE WHERE Id = {0}", purchaseId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    command.CommandText = string.Format("DELETE FROM COMMENT WHERE PurchaseId = {0}", purchaseId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    transaction.Commit();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"DeletePurchase\": {0}", ex.Message);
            }

            return result;
        }
        [Obsolete("For test purposes only!!!")]
        public async Task<bool> DeletePurchase(string purchaseName)
        {
            bool result = false;
            purchaseName = StringUtilities.EscapeStringForDatabase(purchaseName);
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT Id FROM PRODUCT WHERE Name='{0}'", purchaseName);
                    var productId = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0L);
                    if (productId != 0)
                    {
                        command.CommandText = string.Format("DELETE FROM PURCHASE WHERE ProductId='{0}'", productId);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                    
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"DeletePurchase\": {0}", ex.Message);
            }

            return result;
        }
    }
}

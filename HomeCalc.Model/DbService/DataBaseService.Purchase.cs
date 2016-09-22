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
        public async Task<bool> SavePurchase(PurchaseModel purchase)
        {
            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var transaction = db.Connection.BeginTransaction())
                using (var command = db.Connection.CreateCommand())
                {
                    if (purchase.Id == 0)
                    {
                        command.CommandText = string.Format(
                        "INSERT INTO PURCHASE(ProductId, Timestamp, TotalCost, ItemCost, ItemsNumber, StoreId, CommentId) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                        purchase.ProductId,
                        purchase.Timestamp,
                        purchase.TotalCost.ToString(formatCulture),
                        purchase.ItemCost.ToString(formatCulture),
                        purchase.ItemsNumber.ToString(formatCulture),
                        purchase.StoreId,
                        purchase.CommentId);
                    }
                    else
                    {
                        command.CommandText = string.Format(
                        "UPDATE PURCHASE SET ProductId = {0}, Timestamp = {1}, TotalCost = {2}, ItemCost = {3}, ItemsNumber = {4}, StoreId = {5}, CommentId = {6} WHERE Id = {7}",
                        purchase.ProductId,
                        purchase.Timestamp,
                        purchase.TotalCost.ToString(formatCulture),
                        purchase.ItemCost.ToString(formatCulture),
                        purchase.ItemsNumber.ToString(formatCulture),
                        purchase.StoreId,
                        purchase.CommentId);
                    }

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    transaction.Commit();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"SavePurchase\": {0}", ex.Message);
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
                        "INSERT INTO PURCHASE(PurchaseId, Timestamp, TotalCost, ItemCost, ItemNumber, StoreId) VALUES ('{0}', {1}, {2}, {3}, {4}, {5})",
                        purchase.Id, purchase.Timestamp, purchase.TotalCost.ToString(formatCulture), purchase.ItemCost.ToString(formatCulture), purchase.ItemsNumber.ToString(formatCulture), purchase.StoreId);

                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
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
                    string queue = "SELECT pu.Id AS Id, pu.Timestamp, pu.TotalCost, pu.ItemCost, pu.ItemsNumber,"+
                                    "p.Id AS ProductId, p.Name AS ProductName, p.TypeId, p.SubTypeId, p.IsMonthly, "+
                                    "s.Id AS StoreId, s.Name AS StoreName, "+
                                    "c.Id AS CommentId, c.Text AS Comment, c.Rate "+
                    "FROM PURCHASE pu "+
                    "JOIN PRODUCT p ON PU.ProductId=p.Id "+
                    "JOIN STORE s ON PU.StoreId=s.Id "+
                    "JOIN COMMENT c ON PU.CommentId=c.Id "+
                    "WHERE";
                    if (filter.SearchById)
                    {
                        queue = string.Format("{0} Id={1} ", queue, filter.PurchaseId);
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
                        queue = string.Format("{0} CommentID={1}' ", queue, filter.CommentId);
                    }
                    if (filter.SearchByStoreId)
                    {
                        queue = string.Format("{0} StoreID={1}' ", queue, filter.StoreId);
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
                            Id = dataReader.GetInt64(dataReader.GetOrdinal("Id")),
                            ProductName = dataReader.GetString(dataReader.GetOrdinal("ProductName")),
                            ProductId = dataReader.GetInt64(dataReader.GetOrdinal("ProductId")),
                            Timestamp = dataReader.GetInt64(dataReader.GetOrdinal("Timestamp")),
                            TotalCost = dataReader.GetDouble(dataReader.GetOrdinal("TotalCost")),
                            ItemCost = dataReader.GetDouble(dataReader.GetOrdinal("ItemCost")),
                            ItemsNumber = dataReader.GetDouble(dataReader.GetOrdinal("ItemsNumber")),
                            Rate = dataReader.GetInt32(dataReader.GetOrdinal("Rate")),
                            CommentId = dataReader.GetInt64(dataReader.GetOrdinal("CommentId")),
                            Comment = dataReader.GetString(dataReader.GetOrdinal("Comment")),
                            IsMonthly = dataReader.GetBoolean(dataReader.GetOrdinal("IsMonthly")),
                            StoreId = dataReader.GetInt32(dataReader.GetOrdinal("StoreId")),
                            StoreName = dataReader.GetString(dataReader.GetOrdinal("StoreName"))
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
    }
}

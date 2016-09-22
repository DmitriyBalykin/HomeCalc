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
        #region Product
        public async Task<long> SaveProduct(ProductModel product)
        {
            long productId = -1;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    if (product.Id == 0)
                    {
                        command.CommandText = string.Format(
                        "INSERT INTO PRODUCT(Name, TypeId, SubTypeId, IsMonthly) VALUES ('{0}', {1}, {2}, {3}); SELECT last_insert_rowid()",
                        product.Name, product.TypeId, product.SubTypeId, product.IsMonthly);
                        productId = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false));

                    }
                    else
                    {
                        command.CommandText = string.Format(
                        "UPDATE PRODUCT SET Name = '{0}', TypeId = {1}, SubTypeId = {2}, IsMonthly = {3} WHERE Id = {4}",
                        product.Name, product.TypeId, product.SubTypeId, product.IsMonthly, product.Id);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        productId = product.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"SaveProduct\": {0}", ex.Message);
            }
            return productId;
        }
        public async Task<ProductModel> LoadProduct(long id)
        {
            ProductModel product = null;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM PRODUCT WHERE Id = {0}", id);
                    var dbReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    if (dbReader.HasRows && dbReader.Read())
                    {
                        product = new ProductModel
                        {
                            Id = dbReader.GetInt64(0),
                            Name = dbReader.GetString(1),
                            TypeId = dbReader.GetInt32(2),
                            SubTypeId = dbReader.GetInt32(3),
                            IsMonthly = dbReader.GetBoolean(4)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadProduct\": {0}", ex.Message);
            }
            return product;
        }
        public async Task<List<ProductModel>> LoadProductList(SearchRequestModel filter)
        {
            var list = new List<ProductModel>();

            try
            {
                logger.Debug("DatabaseService.LoadProductList: Loading product list");

                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    string queue = "SELECT * FROM PRODUCT WHERE";

                    if (filter.SearchByName)
                    {
                        queue = string.Format("{0} Name LIKE '%{1}%' ", queue, filter.Name.Trim(' '));
                    }
                    if (filter.SearchByType)
                    {
                        queue = string.Format("{0} TypeId = {1} ", queue, filter.TypeId);
                    }
                    if (filter.SearchBySubType)
                    {
                        queue = string.Format("{0} SubTypeId = {1} ", queue, filter.SubTypeId);
                    }
                    if (filter.SearchByMonthly)
                    {
                        queue = string.Format("{0} IsMonthly = {1} ", queue, filter.IsMonthly);
                    }

                    command.CommandText = queue
                        .TrimEnd(" WHERE")
                        .TrimEnd(' ')
                        .Replace("  ", " AND ");

                    var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                    while (dataReader.Read())
                    {
                        list.Add(new ProductModel
                        {
                            Id = dataReader.GetInt64(0),
                            Name = dataReader.GetString(1),
                            TypeId = dataReader.GetInt32(2),
                            SubTypeId = dataReader.GetInt32(3),
                            IsMonthly = dataReader.GetBoolean(4)
                        });
                    }

                    logger.Debug("DatabaseService.LoadProductList: data fetched succesfully");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadProductList\": {0}", ex.Message);
            }
            return list;
        }
        public async Task<bool> DeleteProduct(long productId)
        {
            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM PRODUCT WHERE Id = {0}", productId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"DeleteProduct\": {0}", ex.Message);
            }

            return result;
        }
        #endregion

        #region Type
        public async Task<bool> SaveProductType(ProductTypeModel productType)
        {
            return await SaveProductType(productType, null);
        }
        public async Task<bool> SaveProductType(ProductTypeModel productType, SQLiteCommand transactionCommand)
        {

            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = transactionCommand ?? db.Connection.CreateCommand())
                {
                    if (productType.TypeId == 0)
                    {
                        command.CommandText = string.Format("INSERT INTO PRODUCTTYPE (Name) VALUES ('{0}')", productType.Name);
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE PRODUCTTYPE SET Name = '{0}' WHERE TypeId = {1}", productType.Name, productType.TypeId);
                    }
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"SaveProductType\": {0}", ex.Message);
            }
            return result;
        }
        public async Task<IEnumerable<ProductTypeModel>> LoadProductTypeList()
        {
            var list = new List<ProductTypeModel>();
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM PRODUCTTYPE";
                    var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    while (dataReader.Read())
                    {
                        list.Add(new ProductTypeModel
                        {
                            TypeId = dataReader.GetInt64(0),
                            Name = dataReader.GetString(1)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadProductTypeList\": {0}", ex.Message);
            }
            return list;
        }
        public async Task<bool> DeleteProductType(ProductTypeModel type)
        {
            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM PRODUCTTYPE WHERE Id = {0}", type.TypeId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"DeleteProductType\": {0}", ex.Message);
            }

            return result;
        }
        #endregion

        #region SubType
        public async Task<bool> SaveProductSubType(ProductSubTypeModel productSubType)
        {

            bool result = false;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    if (productSubType.Id == 0)
                    {
                        command.CommandText = string.Format("INSERT INTO PRODUCTSUBTYPE (Name) VALUES ('{0}')", productSubType.Name);
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE PRODUCTSUBTYPE SET Name = '{0}' WHERE Id = {1}", productSubType.Name, productSubType.Id);
                    }
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"SaveProductSubType\": {0}", ex.Message);
            }
            return result;
        }
        public async Task<IEnumerable<ProductSubTypeModel>> LoadProductSubTypeList()
        {
            var list = new List<ProductSubTypeModel>();
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM PRODUCTSUBTYPE";
                    var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    while (dataReader.Read())
                    {
                        list.Add(new ProductSubTypeModel
                        {
                            Id = dataReader.GetInt64(0),
                            Name = dataReader.GetString(1)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadProductSubTypeList\": {0}", ex.Message);
            }
            return list;
        }
        public async Task<bool> DeleteProductSubType(ProductSubTypeModel subType, StorageConnection connection = null)
        {
            bool result = false;
            try
            {
                using (var db = connection ?? dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM PURCHASESUBTYPE WHERE Id = {0}", subType.Id);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"DeleteProductSubType\": {0}", ex.Message);
            }

            return result;
        }
        #endregion

    }
}

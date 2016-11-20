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
                long typeId = await SaveProductType(product.Type);
                
                long subTypeId = 0;
                if (product.SubType != null)
                {
                    product.SubType.TypeId = typeId;
                    subTypeId = await SaveProductSubType(product.SubType);
                }

                using (var db = dbManager.GetConnection())
                using (var transaction = db.Connection.BeginTransaction())
                using (var command = db.Connection.CreateCommand())
                {

                    if (product.Id == 0)
                    {
                        command.CommandText = string.Format("SELECT Id FROM PRODUCT WHERE Name='{0}'", product.Name);
                        productId = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0L);
                    }
                    else
                    {
                        productId = product.Id;
                    }
                    if (productId == 0)
                    {
                        command.CommandText = string.Format(
                        "INSERT INTO PRODUCT(Name, TypeId, SubTypeId, IsMonthly) VALUES ('{0}', {1}, {2}, '{3}'); SELECT last_insert_rowid() FROM PRODUCT",
                        product.Name, typeId, subTypeId, product.IsMonthly);
                        productId = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false));
                        logger.Debug("Saving new product with name={0} and id={1}", product.Name, productId);
                    }
                    else
                    {
                        command.CommandText = string.Format(
                        "UPDATE PRODUCT SET Name = '{0}', TypeId = {1}, SubTypeId = {2}, IsMonthly = '{3}' WHERE Id = {4}",
                        product.Name, typeId, subTypeId, product.IsMonthly, productId);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        logger.Debug("Updating existed product with name={0} and id={1}", product.Name, productId);
                    }
                    transaction.Commit();
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
            return (await LoadProductList(new SearchRequestModel { SearchByProductId = true, ProductId = id }).ConfigureAwait(false)).FirstOrDefault();
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
                    string queue = "SELECT p.Id as ProductId, p.Name as ProductName, p.IsMonthly, t.Id as TypeId, t.Name as TypeName, st.Id as SubTypeId, st.TypeId as StTypeId, st.Name as SubTypeName"+
                    "FROM PRODUCT p"+
                    "JOIN PRODUCTTYPE t ON p.TypeId = t.Id" +
                    "LEFT JOIN PRODUCTSUBTYPE st on p.SubTypeId = st.Id" +
                    "WHERE";
                    if (filter.SearchByProductId)
                    {
                        queue = string.Format("{0} Id = {1} ", queue, filter.ProductId);
                    }
                    if (filter.SearchByName)
                    {
                        queue = string.Format("{0} Name LIKE '%{1}%' ", queue, StringUtilities.EscapeStringForDatabase(filter.Name.Trim(' ')));
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
                            Id = dataReader.GetInt64(dataReader.GetOrdinal("ProductId")),
                            Name = dataReader.GetString(dataReader.GetOrdinal("ProductName")),
                            Type = new ProductTypeModel
                            {
                                TypeId = dataReader.GetInt64(dataReader.GetOrdinal("TypeId")),
                                Name = dataReader.GetString(dataReader.GetOrdinal("TypeName"))
                            },
                            SubType = new ProductSubTypeModel
                            {
                                Id = (dataReader.GetValue(dataReader.GetOrdinal("SubTypeId")) as long?) ?? 0L,
                                TypeId = (dataReader.GetValue(dataReader.GetOrdinal("StTypeId")) as long?) ?? 0L,
                                Name = (dataReader.GetValue(dataReader.GetOrdinal("SubTypeName")) as string) ?? string.Empty
                            },
                            IsMonthly = dataReader.GetBoolean(dataReader.GetOrdinal("IsMonthly"))
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
                    logger.Debug("Deleting product with id={1}", productId);
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
        public async Task<long> SaveProductType(ProductTypeModel productType)
        {
            long typeId = productType.TypeId;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    if (typeId == 0)
                    {
                        //is this part really needed?
                        command.CommandText = string.Format("SELECT TypeId FROM PRODUCTTYPE WHERE Name='{0}'", productType.Name);
                        typeId = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0L);
                    }
                    if (typeId == 0)
                    {
                        command.CommandText = string.Format("INSERT INTO PRODUCTTYPE (Name) VALUES ('{0}'); SELECT last_insert_rowid() FROM PRODUCTTYPE", productType.Name);
                        typeId = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false));
                        logger.Debug("Saving new product type with name={0} and id={1}", productType.Name, typeId);
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE PRODUCTTYPE SET Name = '{0}' WHERE TypeId = {1}", productType.Name, productType.TypeId);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        logger.Debug("Updating existed product type with name={0} and id={1}", productType.Name, typeId);
                    }
                }
            }
            catch (Exception ex)
            {
                typeId = -1;
                logger.Error("Exception during execution method \"SaveProductType\": {0}", ex.Message);
            }
            return typeId;
        }
        public async Task<ProductTypeModel> LoadProductType(long typeId)
        {
            ProductTypeModel type = null;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM PRODUCTTYPE WHERE Id={0}", typeId);
                    var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    if (dataReader.Read())
                    {
                        type = new ProductTypeModel
                        {
                            TypeId = dataReader.GetInt64(0),
                            Name = dataReader.GetString(1)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadProductType\": {0}", ex.Message);
            }
            return type;
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
        public async Task<long> SaveProductSubType(ProductSubTypeModel productSubType)
        {
            long subTypeId = productSubType.Id;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    if (subTypeId == 0)
                    {
                        //is this part really needed?
                        command.CommandText = string.Format("SELECT Id FROM PRODUCTSUBTYPE WHERE Name='{0}'", productSubType.Name);
                        subTypeId = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0L);
                    }
                    if (subTypeId == 0)
                    {
                        command.CommandText = string.Format("INSERT INTO PRODUCTSUBTYPE (Name, TypeId) VALUES ('{0}', {1});SELECT last_insert_rowid() FROM PRODUCTSUBTYPE", productSubType.Name, productSubType.TypeId);
                        subTypeId = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false));
                        logger.Debug("Saving new product sub type with name={0} and id={1}", productSubType.Name, subTypeId);
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE PRODUCTSUBTYPE SET Name = '{0}', TypeId = {1} WHERE Id = {2}", productSubType.Name, productSubType.TypeId, productSubType.Id);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        logger.Debug("Updating existed product sub type with name={0} and id={1}", productSubType.Name, subTypeId);
                    }
                }
            }
            catch (Exception ex)
            {
                subTypeId = -1;
                logger.Error("Exception during execution method \"SaveProductSubType\": {0}", ex.Message);
            }
            return subTypeId;
        }
        public async Task<ProductSubTypeModel> LoadProductSubType(long subTypeId)
        {
            ProductSubTypeModel subType = null;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM PRODUCTSUBTYPE WHERE Id={0}", subTypeId);
                    var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    if (dataReader.Read())
                    {
                        subType = new ProductSubTypeModel
                        {
                            Id = dataReader.GetInt64(0),
                            Name = dataReader.GetString(1)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadProductType\": {0}", ex.Message);
            }
            return subType;
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
                            Name = dataReader.GetString(1),
                            TypeId = dataReader.GetInt64(2)
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
                    command.CommandText = string.Format("DELETE FROM PRODUCTSUBTYPE WHERE Id = {0}", subType.Id);
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

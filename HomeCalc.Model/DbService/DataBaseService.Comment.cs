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
        public async Task<long> SaveComment(CommentModel comment)
        {
            long commentId = -1;
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    if (comment.Id == 0)
	                {
                        command.CommandText = string.Format("INSERT INTO COMMENT(PurchaseId, StoreId, Text, Rate) VALUES ({0}, '{1}', '{2}', '{3}'); SELECT last_insert_rowid()",
                            comment.PurchaseId, comment.StoreId, comment.Text, comment.Rate);
                        commentId = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false));
                    }
                    else
                    {
                        command.CommandText = string.Format(
                            "UPDATE COMMENT SET PurchaseId = {0}, StoreId = '{1}', Text = '{2}', Rate = '{3}' WHERE Id = {4}",
                            comment.PurchaseId, comment.StoreId, comment.Text, comment.Rate, comment.Id);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        commentId = comment.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"SaveComment\": {0}", ex.Message);
            }

            return commentId;
        }
        public async Task<IEnumerable<CommentModel>> LoadComments(long commentId, long purchaseId, long storeId)
        {
            var comments = new List<CommentModel>();
            try
            {
                using (var db = dbManager.GetConnection())
                using (var command = db.Connection.CreateCommand())
                {
                    string queue = string.Format("SELECT * FROM COMMENT WHERE");

                    if (commentId != 0)
                    {
                        queue = string.Format("{0} Id={1} ", queue, commentId);
                    }
                    if (purchaseId != 0)
                    {
                        queue = string.Format("{0} PurchaseId={1} ", queue, purchaseId);
                    }
                    if (storeId != 0)
                    {
                        queue = string.Format("{0} StoreId={1} ", queue, storeId);
                    }

                    command.CommandText = queue
                        .TrimEnd(" WHERE")
                        .TrimEnd(' ')
                        .Replace("  ", " AND ");

                    DbDataReader dbDataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                    while (dbDataReader.HasRows && dbDataReader.Read())
                    {
                        comments.Add(new CommentModel()
                        {
                            Id = dbDataReader.GetInt64(0),
                            PurchaseId = dbDataReader.GetInt64(1),
                            StoreId = dbDataReader.GetInt64(2),
                            Text = dbDataReader.GetString(3)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception during execution method \"LoadComments\": {0}", ex.Message);
            }

            return comments;
        }
        public async Task<bool> DeleteComment(long commentId)
        {
            bool result = false;
            try
            {
                using(var db = dbManager.GetConnection())
                using(var command = db.Connection.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM COMMENT WHERE Id = {0}", commentId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error("Exception during execution method \"DeleteComment\": {0}", ex.Message);
            }
            
            return result;
        }
 
    }
}

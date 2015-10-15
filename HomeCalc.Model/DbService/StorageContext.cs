using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DbService
{
    public class StorageContext: DbContext, IDisposable
    {
        public StorageContext(DbConnection connection, bool contextOwnConnection = true)
            : base(connection, contextOwnConnection)
        { Initiated = false;  }
        public DbSet<PurchaseTypeModel> PurchaseType { get; set; }
        public DbSet<PurchaseModel> Purchase { get; set; }
        public bool Initiated { get; set; }
        void IDisposable.Dispose()
        {
            if (Database.Connection.State != System.Data.ConnectionState.Closed)
            {
                Database.Connection.Close();    
            }
       }
    }
}

using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DbService
{
    public class StorageContext: DbContext
    {
        public DbSet<PurchaseTypeModel> PurchaseType { get; set; }
        public DbSet<PurchaseModel> Purchase { get; set; }
    }
}

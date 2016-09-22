using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DataModels
{
    public class PurchaseModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public long Timestamp { get; set; }
        public double TotalCost { get; set; }
        public double ItemCost { get; set; }
        public double ItemsNumber { get; set; }
        public int Rate { get; set; }
        public long CommentId { get; set; }
        public string Comment { get; set; }
        public long StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreComment { get; set; }
        public int StoreRate { get; set; }
        public bool IsMonthly { get; set; }
        public long TypeId { get; set; }
        public long SubTypeId { get; set; }
    }

}

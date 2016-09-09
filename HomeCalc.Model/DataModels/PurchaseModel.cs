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
        public Int64 PurchaseId { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public int SubTypeId { get; set; }
        public bool IsMonthly { get; set; }
    }
}

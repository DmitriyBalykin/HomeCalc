using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DataModels
{
    public class ProductModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public long TypeId { get; set; }
        public long SubTypeId { get; set; }
        public bool IsMonthly { get; set; }
    }
}

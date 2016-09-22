using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DataModels
{
    public class ProductTypeModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 TypeId { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return (obj is ProductTypeModel) && ((ProductTypeModel)obj).TypeId == this.TypeId;
        }

        public override int GetHashCode()
        {
            return (int)this.TypeId;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}

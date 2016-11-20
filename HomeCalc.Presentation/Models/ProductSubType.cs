using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeCalc.Presentation.Models
{
    public class ProductSubType
    {
        public long Id { get; set; }
        public long TypeId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj != null && (obj is ProductSubType) && ((ProductSubType)obj).Name == Name;
        }

        public override int GetHashCode()
        {
            return (int)Id;
        }
    }
}

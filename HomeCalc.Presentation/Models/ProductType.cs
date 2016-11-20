using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeCalc.Presentation.Models
{
    public class ProductType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
        public override bool Equals(object obj)
        {
            return obj != null && (obj is ProductType) && ((ProductType)obj).Name == Name;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeCalc.Presentation.Models
{
    public class ProductType
    {
        public int TypeId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
